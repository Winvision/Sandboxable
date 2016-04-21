using System;
using System.Net;
using System.Net.Http;
using Sandboxable.Hyak.Common.Internals;

namespace Sandboxable.Hyak.Common
{
    public class CloudHttpResponseErrorInfo : CloudHttpErrorInfo
    {
        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the reason phrase which typically is sent by servers together
        /// with the status code.
        /// </summary>
        public string ReasonPhrase
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a new CloudHttpResponseErrorInfo from a HttpResponseMessage.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>A CloudHttpResponseErrorInfo instance.</returns>
        public static CloudHttpResponseErrorInfo Create(HttpResponseMessage response)
        {
            return Create(response, response.Content.AsString());
        }

        /// <summary>
        /// Creates a new CloudHttpResponseErrorInfo from a HttpResponseMessage.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <param name="content">
        /// The response content, which may be passed separately if the
        /// response has already been disposed.
        /// </param>
        /// <returns>A CloudHttpResponseErrorInfo instance.</returns>
        public static CloudHttpResponseErrorInfo Create(HttpResponseMessage response, string content)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var cloudHttpResponseErrorInfo = new CloudHttpResponseErrorInfo
            {
                Content = content,
                Version = response.Version
            };

            cloudHttpResponseErrorInfo.CopyHeaders(response.Headers);
            cloudHttpResponseErrorInfo.CopyHeaders(response.GetContentHeaders());
            cloudHttpResponseErrorInfo.StatusCode = response.StatusCode;
            cloudHttpResponseErrorInfo.ReasonPhrase = response.ReasonPhrase;

            return cloudHttpResponseErrorInfo;
        }
    }
}