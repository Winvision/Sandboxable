using System;
using System.Net.Http;

namespace Sandboxable.Hyak.Common.Platform
{
    public interface IHttpTransportHandlerProvider
    {
        HttpMessageHandler CreateHttpTransportHandler();
    }
}