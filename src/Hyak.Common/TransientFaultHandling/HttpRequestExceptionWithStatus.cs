using System;
using System.Net;
using System.Net.Http;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    public class HttpRequestExceptionWithStatus : HttpRequestException
    {
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public HttpRequestExceptionWithStatus()
        {
        }

        public HttpRequestExceptionWithStatus(string message) 
            : base(message)
        {
        }

        public HttpRequestExceptionWithStatus(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}