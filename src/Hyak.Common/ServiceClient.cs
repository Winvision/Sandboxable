using Sandboxable.Hyak.Common.Internals;
using Sandboxable.Hyak.Common.Platform;
using Sandboxable.Hyak.Common.Properties;
using Sandboxable.Hyak.Common.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// The base ServiceClient class used to call REST services.
    /// </summary>
    /// <typeparam name="T">Type of the ServiceClient.</typeparam>
    public abstract class ServiceClient<T> : IDisposable
        where T : ServiceClient<T>
    {
        /// <summary>
        /// Gets the Platform's IHttpTransportHandlerProvider which gives the
        /// default HttpHandler for sending web requests.
        /// </summary>
        private static IHttpTransportHandlerProvider _transportHandlerProvider;

        /// <summary>
        /// A value indicating whether or not the ServiceClient has already
        /// been disposed.
        /// </summary>
        internal bool _disposed;

        /// <summary>
        /// Reference to the delegated handler of our handler (so we can
        /// maintain a proper reference count).
        /// </summary>
        internal DisposableReference<HttpMessageHandler> _innerHandler;

        /// <summary>
        /// Reference to our HTTP handler (which is the start of our HTTP
        /// pipeline).
        /// </summary>
        internal DisposableReference<HttpMessageHandler> _handler;

        /// <summary>
        /// Gets the HttpClient used for making HTTP requests.
        /// </summary>
        public HttpClient HttpClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to our HTTP handler (which is the start of our
        /// HTTP pipeline).
        /// </summary>
        protected internal HttpMessageHandler HttpMessageHandler => this._handler.Reference;

        /// <summary>
        /// Gets the UserAgent collection which can be augmented with custom
        /// user agent strings.
        /// </summary>
        public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent => this.HttpClient.DefaultRequestHeaders.UserAgent;

        /// <summary>
        /// Initializes static members of the ServiceClient class.
        /// </summary>
        static ServiceClient()
        {
            _transportHandlerProvider = PortablePlatformAbstraction.Get<IHttpTransportHandlerProvider>(true);
        }

        /// <summary>
        /// Initializes a new instance of the ServiceClient class.
        /// </summary>
        public ServiceClient()
        {
            this.InitializeHttpClient(_transportHandlerProvider.CreateHttpTransportHandler());
        }

        /// <summary>
        /// Initializes a new instance of the ServiceClient class.
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        public ServiceClient(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Add a handler to the end of the client's HTTP pipeline.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void AddHandlerToPipeline(DelegatingHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var disposableReference = this._innerHandler;
            var disposableReference1 = this._handler;
            var disposableReference2 = new DisposableReference<HttpMessageHandler>(handler);
            if (disposableReference != null)
            {
                this._innerHandler = null;
                disposableReference.ReleaseReference();
                disposableReference = null;
            }
            handler.InnerHandler = new IndisposableDelegatingHandler(disposableReference1.Reference);
            disposableReference1.AddReference();

            this._innerHandler = disposableReference1;
            this._handler = disposableReference2;

            var httpClient = this.HttpClient;
            this.HttpClient = new HttpClient(handler, false);

            CloneHttpClient(httpClient, this.HttpClient);
        }

        /// <summary>
        /// Clone the service client.
        /// </summary>
        /// <param name="client">The client to clone.</param>
        protected virtual void Clone(ServiceClient<T> client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            CloneHttpClient(this.HttpClient, client.HttpClient);
        }

        /// <summary>
        /// Clone HttpClient properties.
        /// </summary>
        /// <param name="source">The client to clone.</param>
        /// <param name="destination">The client to copy into.</param>
        internal static void CloneHttpClient(HttpClient source, HttpClient destination)
        {
            destination.Timeout = source.Timeout;
            destination.MaxResponseContentBufferSize = source.MaxResponseContentBufferSize;
            destination.BaseAddress = source.BaseAddress;
            destination.DefaultRequestHeaders.UserAgent.Clear();

            foreach (var userAgent in source.DefaultRequestHeaders.UserAgent)
            {
                destination.DefaultRequestHeaders.UserAgent.Add(userAgent);
            }

            foreach (var keyValuePair in source.DefaultRequestHeaders.Where(p => p.Key != "User-Agent"))
            {
                destination.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Create the HTTP client.
        /// </summary>
        /// <returns>The HTTP client.</returns>
        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(this._handler.Reference, false);
            var type = this.GetType();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(type.FullName, this.GetAssemblyVersion()));
            return httpClient;
        }

        /// <summary>
        /// Dispose the ServiceClient.
        /// </summary>
        public virtual void Dispose()
        {
            if (!this._disposed)
            {
                this._disposed = true;
                this.HttpClient.Dispose();
                this.HttpClient = null;
                if (this._innerHandler != null)
                {
                    this._innerHandler.ReleaseReference();
                    this._innerHandler = null;
                }
                this._handler.ReleaseReference();
                this._handler = null;
            }
        }

        /// <summary>
        /// Get the assembly version of a service client.
        /// </summary>
        /// <returns>The assembly version of the client.</returns>
        private string GetAssemblyVersion()
        {
            return this.GetType().Assembly.FullName
                .Split(',')
                .Select(c => c.Trim())
                .FirstOrDefault(c => c.StartsWith("Version="))
                ?.Substring("Version=".Length);
    }

        /// <summary>
        /// Get the HTTP pipeline for the given service client.
        /// </summary>
        /// <returns>The client's HTTP pipeline.</returns>
        public IEnumerable<HttpMessageHandler> GetHttpPipeline()
        {
            HttpMessageHandler innerHandler;

            for (var i = this.HttpMessageHandler; i != null; i = innerHandler)
            {
                yield return i;

                var delegatingHandler = i as DelegatingHandler;

                innerHandler = delegatingHandler?.InnerHandler;
            }
        }

        /// <summary>
        /// Initializes HttpClient.
        /// </summary>
        /// <param name="httpMessageHandler">Http message handler to use with Http client.</param>
        protected void InitializeHttpClient(HttpMessageHandler httpMessageHandler)
        {
            this._handler = new DisposableReference<HttpMessageHandler>(httpMessageHandler);
            this.HttpClient = this.CreateHttpClient();
            this.AddHandlerToPipeline(new RetryHandler());
        }

        /// <summary>
        /// Sets retry policy for the client.
        /// </summary>
        /// <param name="retryPolicy">Retry policy to set.</param>
        public void SetRetryPolicy(RetryPolicy retryPolicy)
        {
            if (retryPolicy == null)
            {
                throw new ArgumentNullException(nameof(retryPolicy));
            }

            var retryHandler = this.GetHttpPipeline().OfType<RetryHandler>().FirstOrDefault();
            if (retryHandler == null)
            {
                throw new InvalidOperationException(Resources.ExceptionRetryHandlerMissing);
            }

            retryHandler.RetryPolicy = retryPolicy;
        }

        public virtual T WithHandler(DelegatingHandler handler)
        {
            return this.WithHandler(Activator.CreateInstance(typeof(T)) as T, handler);
        }

        /// <summary>
        /// Extend the ServiceClient with a new handler.
        /// </summary>
        /// <param name="newClient">The new client that will extend.</param>
        /// <param name="handler">The handler to extend with.</param>
        /// <returns>The extended client.</returns>
        protected virtual T WithHandler(ServiceClient<T> newClient, DelegatingHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (newClient == null)
            {
                throw new ArgumentNullException(nameof(newClient));
            }

            newClient._handler = new DisposableReference<HttpMessageHandler>(handler);
            newClient._innerHandler = this._handler;
            this._handler.AddReference();
            handler.InnerHandler = new IndisposableDelegatingHandler(this._handler.Reference);
            newClient.HttpClient = new HttpClient(handler, false);
            this.Clone(newClient);
            return (T)newClient;
        }

        public T WithHandlers(IEnumerable<DelegatingHandler> handlers)
        {
            var obj1 = (T)this;
            foreach (var handler in handlers)
            {
                var obj2 = obj1.WithHandler(handler);
                if (obj1 != this)
                {
                    obj1.Dispose();
                }
                obj1 = obj2;
            }
            return obj1;
        }
    }
}