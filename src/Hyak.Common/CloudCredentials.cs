using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sandboxable.Hyak.Common
{
    public abstract class CloudCredentials
    {
        /// <summary>
        /// Apply the credentials to the HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// Task that will complete when processing has completed.
        /// </returns>
        public virtual Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public virtual void InitializeServiceClient<T>(ServiceClient<T> client)
            where T : ServiceClient<T>
        {
        }
    }
}