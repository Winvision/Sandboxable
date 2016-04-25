using System;
using System.Net;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    public class DefaultHttpErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            var httpRequestExceptionWithStatus = ex as HttpRequestExceptionWithStatus;
            if (httpRequestExceptionWithStatus != null && (httpRequestExceptionWithStatus.StatusCode == HttpStatusCode.RequestTimeout 
                                                           || httpRequestExceptionWithStatus.StatusCode >= HttpStatusCode.InternalServerError 
                                                           && httpRequestExceptionWithStatus.StatusCode != HttpStatusCode.NotImplemented 
                                                           && httpRequestExceptionWithStatus.StatusCode != HttpStatusCode.HttpVersionNotSupported))
            {
                return true;
            }

            return false;
        }
    }
}