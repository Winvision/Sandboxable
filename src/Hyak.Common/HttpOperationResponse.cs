using System;
using System.Net;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// A standard service response including an HTTP status code and request
    /// ID.
    /// </summary>
    public class HttpOperationResponse
    {
        /// <summary>
        /// Gets or sets the standard HTTP status code from the REST API 
        /// operations for the Service Management API.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }
    }
}