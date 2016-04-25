using System;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    /// <summary>
    /// A retry strategy with backoff parameters for calculating the exponential delay between retries.
    /// </summary>
    public class ExponentialBackoff : RetryStrategy
    {
        private readonly int _retryCount;

        private readonly TimeSpan _minBackoff;

        private readonly TimeSpan _maxBackoff;

        private readonly TimeSpan _deltaBackoff;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.TransientFaultHandling.ExponentialBackoff" /> class. 
        /// </summary>
        public ExponentialBackoff() : this(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultClientBackoff)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.TransientFaultHandling.ExponentialBackoff" /> class with the specified retry settings.
        /// </summary>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum backoff time</param>
        /// <param name="maxBackoff">The maximum backoff time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        public ExponentialBackoff(int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) 
            : this(null, retryCount, minBackoff, maxBackoff, deltaBackoff, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.TransientFaultHandling.ExponentialBackoff" /> class with the specified name and retry settings.
        /// </summary>
        /// <param name="name">The name of the retry strategy.</param>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum backoff time</param>
        /// <param name="maxBackoff">The maximum backoff time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        public ExponentialBackoff(string name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) 
            : this(name, retryCount, minBackoff, maxBackoff, deltaBackoff, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.TransientFaultHandling.ExponentialBackoff" /> class with the specified name, retry settings, and fast retry option.
        /// </summary>
        /// <param name="name">The name of the retry strategy.</param>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum backoff time</param>
        /// <param name="maxBackoff">The maximum backoff time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        public ExponentialBackoff(string name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, bool firstFastRetry) 
            : base(name, firstFastRetry)
        {
            Guard.ArgumentNotNegativeValue(retryCount, "retryCount");
            Guard.ArgumentNotNegativeValue(minBackoff.Ticks, "minBackoff");
            Guard.ArgumentNotNegativeValue(maxBackoff.Ticks, "maxBackoff");
            Guard.ArgumentNotNegativeValue(deltaBackoff.Ticks, "deltaBackoff");
            Guard.ArgumentNotGreaterThan(minBackoff.TotalMilliseconds, maxBackoff.TotalMilliseconds, "minBackoff");
            this._retryCount = retryCount;
            this._minBackoff = minBackoff;
            this._maxBackoff = maxBackoff;
            this._deltaBackoff = deltaBackoff;
        }

        /// <summary>
        /// Returns the corresponding ShouldRetry delegate.
        /// </summary>
        /// <returns>The ShouldRetry delegate.</returns>
        public override ShouldRetry GetShouldRetry()
        {
            return (int currentRetryCount, Exception lastException, out TimeSpan retryInterval) => {
                if (currentRetryCount >= this._retryCount)
                {
                    retryInterval = TimeSpan.Zero;
                    return false;
                }

                var random = new Random();
                var totalMilliseconds = (int)(this._deltaBackoff.TotalMilliseconds * 0.8);
                var timeSpan = this._deltaBackoff;
                var num = (int)((Math.Pow(2, currentRetryCount) - 1) * (double)random.Next(totalMilliseconds, (int)(timeSpan.TotalMilliseconds * 1.2)));
                retryInterval = TimeSpan.FromMilliseconds((int)Math.Min(this._minBackoff.TotalMilliseconds + num, this._maxBackoff.TotalMilliseconds));
                return true;
            };
        }
    }
}