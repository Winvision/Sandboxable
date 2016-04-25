﻿using Sandboxable.Hyak.Common.Properties;
using Sandboxable.Hyak.Common.TransientFaultHandling;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// Http retry handler.
    /// </summary>
    public class RetryHandler : DelegatingHandler
    {
        private readonly TimeSpan DefaultMinBackoff = new TimeSpan(0, 0, 1);

        private readonly TimeSpan DefaultMaxBackoff = new TimeSpan(0, 0, 10);

        private readonly TimeSpan DefaultBackoffDelta = new TimeSpan(0, 0, 10);

        /// <summary>
        /// Gets or sets retry policy.
        /// </summary>
        public RetryPolicy RetryPolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.RetryHandler" /> class. Sets 
        /// default retry policty base on Exponential Backoff.
        /// </summary>
        public RetryHandler()
        {
            var exponentialBackoff = new ExponentialBackoff(3, this.DefaultMinBackoff, this.DefaultMaxBackoff, this.DefaultBackoffDelta);

            this.RetryPolicy = new RetryPolicy<DefaultHttpErrorDetectionStrategy>(exponentialBackoff);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.RetryHandler" /> class. Sets 
        /// the default retry policty base on Exponential Backoff.
        /// </summary>
        /// <param name="innerHandler">Inner http handler.</param>
        public RetryHandler(DelegatingHandler innerHandler) : base(innerHandler)
        {
            var exponentialBackoff = new ExponentialBackoff(3, this.DefaultMinBackoff, this.DefaultMaxBackoff, this.DefaultBackoffDelta);

            this.RetryPolicy = new RetryPolicy<DefaultHttpErrorDetectionStrategy>(exponentialBackoff);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.RetryHandler" /> class. 
        /// </summary>
        /// <param name="retryPolicy">Retry policy to use.</param>
        /// <param name="innerHandler">Inner http handler.</param>
        public RetryHandler(RetryPolicy retryPolicy, DelegatingHandler innerHandler) : base(innerHandler)
        {
            if (retryPolicy == null)
            {
                throw new ArgumentNullException(nameof(retryPolicy));
            }

            this.RetryPolicy = retryPolicy;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous
        /// operation. Retries request if needed based on Retry Policy.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns System.Threading.Tasks.Task&lt;TResult&gt;. The 
        /// task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.RetryPolicy.Retrying += (sender, args) =>
            {
                if (this.Retrying == null)
                {
                    return;
                }

                this.Retrying(sender, args);
            };

            HttpResponseMessage responseMessage = null;

            try
            {
                await this.RetryPolicy.ExecuteAsync(async () =>
                {
                    responseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        throw new HttpRequestExceptionWithStatus(string.Format(CultureInfo.InvariantCulture, Resources.ResponseStatusCodeError, responseMessage.StatusCode, responseMessage.StatusCode))
                        {
                            StatusCode = responseMessage.StatusCode
                        };
                    }
                    return responseMessage;
                }, cancellationToken).ConfigureAwait(false);

                return responseMessage;
            }
            catch
            {
                if (responseMessage != null)
                {
                    return responseMessage;
                }

                throw;
            }
        }

        /// <summary>
        /// An instance of a callback delegate that will be invoked whenever a retry condition is encountered.
        /// </summary>
        public event EventHandler<RetryingEventArgs> Retrying;
    }
}