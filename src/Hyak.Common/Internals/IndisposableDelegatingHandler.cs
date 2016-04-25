using System;
using System.Net.Http;

namespace Sandboxable.Hyak.Common.Internals
{
    /// <summary>
    /// Wrapper class for HttpMessageHandler that prevents InnerHandler from
    /// being disposed.
    /// </summary>
    internal class IndisposableDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.Internals.IndisposableDelegatingHandler" /> class from HttpMessageHandler.
        /// </summary>
        /// <param name="innerHandler">InnerHandler to wrap.</param>
        public IndisposableDelegatingHandler(HttpMessageHandler innerHandler) 
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Overrides Dispose of the base class to prevent disposal of the InnerHandler.
        /// </summary>
        /// <param name="disposing">If set to true indicates the method is being called from Dispose().</param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}