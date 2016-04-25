using System;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    /// <summary>
    /// Represents a retry strategy that determines the number of retry attempts and the interval between retries.
    /// </summary>
    public abstract class RetryStrategy
    {
        /// <summary>
        /// Represents the default number of retry attempts.
        /// </summary>
        public static readonly int DefaultClientRetryCount;

        /// <summary>
        /// Represents the default amount of time used when calculating a random delta in the exponential delay between retries.
        /// </summary>
        public static readonly TimeSpan DefaultClientBackoff;

        /// <summary>
        /// Represents the default maximum amount of time used when calculating the exponential delay between retries.
        /// </summary>
        public static readonly TimeSpan DefaultMaxBackoff;

        /// <summary>
        /// Represents the default minimum amount of time used when calculating the exponential delay between retries.
        /// </summary>
        public static readonly TimeSpan DefaultMinBackoff;

        /// <summary>
        /// Represents the default interval between retries.
        /// </summary>
        public static readonly TimeSpan DefaultRetryInterval;

        /// <summary>
        /// Represents the default time increment between retry attempts in the progressive delay policy.
        /// </summary>
        public static readonly TimeSpan DefaultRetryIncrement;

        /// <summary>
        /// Represents the default flag indicating whether the first retry attempt will be made immediately,
        /// whereas subsequent retries will remain subject to the retry interval.
        /// </summary>
        public static readonly bool DefaultFirstFastRetry;

        /// <summary>
        /// Returns a default policy that implements a random exponential retry interval configured with the <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" />, <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultMinBackoff" />, <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultMaxBackoff" />, and <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultClientBackoff" /> parameters.
        /// The default retry policy treats all caught exceptions as transient errors.
        /// </summary>
        public static RetryStrategy DefaultExponential { get; }

        /// <summary>
        /// Returns a default policy that implements a fixed retry interval configured with the <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" /> and <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultRetryInterval" /> parameters.
        /// The default retry policy treats all caught exceptions as transient errors.
        /// </summary>
        public static RetryStrategy DefaultFixed { get; }

        /// <summary>
        /// Returns a default policy that implements a progressive retry interval configured with the <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultClientRetryCount" />, <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultRetryInterval" />, and <see cref="F:Hyak.Common.TransientFaultHandling.RetryStrategy.DefaultRetryIncrement" /> parameters.
        /// The default retry policy treats all caught exceptions as transient errors.
        /// </summary>
        public static RetryStrategy DefaultProgressive { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the first retry attempt will be made immediately,
        /// whereas subsequent retries will remain subject to the retry interval.
        /// </summary>
        public bool FastFirstRetry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the retry strategy.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a default policy that performs no retries, but invokes the action only once.
        /// </summary>
        public static RetryStrategy NoRetry { get; }

        static RetryStrategy()
        {
            DefaultClientRetryCount = 10;
            DefaultClientBackoff = TimeSpan.FromSeconds(10);
            DefaultMaxBackoff = TimeSpan.FromSeconds(30);
            DefaultMinBackoff = TimeSpan.FromSeconds(1);
            DefaultRetryInterval = TimeSpan.FromSeconds(1);
            DefaultRetryIncrement = TimeSpan.FromSeconds(1);
            DefaultFirstFastRetry = true;
            NoRetry = new FixedInterval(0, DefaultRetryInterval);
            DefaultFixed = new FixedInterval(DefaultClientRetryCount, DefaultRetryInterval);
            DefaultProgressive = new Incremental(DefaultClientRetryCount, DefaultRetryInterval, DefaultRetryIncrement);
            DefaultExponential = new ExponentialBackoff(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultClientBackoff);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hyak.Common.TransientFaultHandling.RetryStrategy" /> class. 
        /// </summary>
        /// <param name="name">The name of the retry strategy.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        protected RetryStrategy(string name, bool firstFastRetry)
        {
            Name = name;
            FastFirstRetry = firstFastRetry;
        }

        /// <summary>
        /// Returns the corresponding ShouldRetry delegate.
        /// </summary>
        /// <returns>The ShouldRetry delegate.</returns>
        public abstract ShouldRetry GetShouldRetry();
    }
}