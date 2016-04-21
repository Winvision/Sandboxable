using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// Base class used to describe HTTP requests and responses associated with
    /// error conditions.
    /// </summary>
    public abstract class CloudHttpErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the CloudHttpErrorInfo class.
        /// </summary>
        protected CloudHttpErrorInfo()
        {
            this.Headers = new Dictionary<string, IEnumerable<string>>();
        }

        /// <summary>
        /// Gets or sets the contents of the HTTP message.
        /// </summary>
        public string Content
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the HTTP message version.
        /// </summary>
        public Version Version
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the collection of HTTP headers.
        /// </summary>
        public IDictionary<string, IEnumerable<string>> Headers
        {
            get;
        }

        /// <summary>
        /// Add the HTTP message headers to the error info.
        /// </summary>
        /// <param name="headers">Collection of HTTP header.</param>
        protected void CopyHeaders(HttpHeaders headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    IEnumerable<string> enumerable;

                    enumerable = !this.Headers.TryGetValue(header.Key, out enumerable) ? header.Value : enumerable.Concat(header.Value);

                    this.Headers.Add(header.Key, enumerable);
                }
            }
        }
    }
}