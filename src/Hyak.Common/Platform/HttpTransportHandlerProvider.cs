using System;
using System.Net.Http;

namespace Sandboxable.Hyak.Common.Platform
{
    public class HttpTransportHandlerProvider : IHttpTransportHandlerProvider
    {
        public HttpTransportHandlerProvider()
        {
        }

        public HttpMessageHandler CreateHttpTransportHandler()
        {
            return new WebRequestHandler();
        }
    }
}
