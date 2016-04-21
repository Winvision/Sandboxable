using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sandboxable.Hyak.Common.Internals
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Get the HTTP message content as a string.
        /// </summary>
        /// <param name="content">The HTTP content.</param>
        /// <returns>The HTTP message content as a string.</returns>
        public static string AsString(this HttpContent content)
        {
            try
            {
                if (content != null)
                {
                    var configuredTaskAwaitable = content.ReadAsStringAsync().ConfigureAwait(false);

                    return configuredTaskAwaitable.GetAwaiter().GetResult();
                }
            }
            catch (ObjectDisposedException)
            {
            }

            return null;
        }

        public static HttpHeaders GetContentHeaders(this HttpRequestMessage request)
        {
            try
            {
                if (request?.Content != null)
                {
                    return request.Content.Headers;
                }
            }
            catch (ObjectDisposedException)
            {
            }

            return null;
        }

        public static HttpHeaders GetContentHeaders(this HttpResponseMessage response)
        {
            try
            {
                if (response?.Content != null)
                {
                    return response.Content.Headers;
                }
            }
            catch (ObjectDisposedException)
            {
            }

            return null;
        }
    }
}