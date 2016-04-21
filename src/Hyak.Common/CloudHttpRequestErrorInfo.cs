using System;
using System.Collections.Generic;
using System.Net.Http;
using Sandboxable.Hyak.Common.Internals;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// Describes HTTP requests associated with error conditions.
    /// </summary>
    public class CloudHttpRequestErrorInfo : CloudHttpErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the CloudHttpRequestErrorInfo class.
        /// </summary>
        protected CloudHttpRequestErrorInfo()
        {
            this.Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the Uri used for the HTTP request.
        /// </summary>
        public Uri RequestUri
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the HTTP method used by the HTTP request message.
        /// </summary>
        public HttpMethod Method
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a set of properties for the HTTP request.
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get;
        }

        public static CloudHttpRequestErrorInfo Create(HttpRequestMessage request, string content)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var cloudHttpRequestErrorInfo = new CloudHttpRequestErrorInfo
            {
                Content = content,
                Version = request.Version
            };

            cloudHttpRequestErrorInfo.CopyHeaders(request.Headers);
            cloudHttpRequestErrorInfo.CopyHeaders(request.GetContentHeaders());
            cloudHttpRequestErrorInfo.Method = request.Method;
            cloudHttpRequestErrorInfo.RequestUri = request.RequestUri;

            if (request.Properties != null)
            {
                foreach (var property in request.Properties)
                {
                    cloudHttpRequestErrorInfo.Properties.Add(property.Key, property.Value);
                }
            }

            return cloudHttpRequestErrorInfo;
        }
    }
}