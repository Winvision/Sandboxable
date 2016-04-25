using System;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    public class RetryPolicy<T> : RetryPolicy
        where T : ITransientErrorDetectionStrategy, new()
    {
        public RetryPolicy(RetryStrategy retryStrategy) 
            : base(default(T) == null ? Activator.CreateInstance<T>() : default(T), retryStrategy)
        {
        }

        public RetryPolicy(int retryCount) 
            : base(default(T) == null ? Activator.CreateInstance<T>() : default(T), retryCount)
        {
        }

        public RetryPolicy(int retryCount, TimeSpan retryInterval) 
            : base(default(T) == null ? Activator.CreateInstance<T>() : default(T), retryCount, retryInterval)
        {
        }

        public RetryPolicy(int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) 
            : base(default(T) == null ? Activator.CreateInstance<T>() : default(T), retryCount, minBackoff, maxBackoff, deltaBackoff)
        {
        }

        public RetryPolicy(int retryCount, TimeSpan initialInterval, TimeSpan increment) 
            : base(default(T) == null ? Activator.CreateInstance<T>() : default(T), retryCount, initialInterval, increment)
        {
        }
    }
}