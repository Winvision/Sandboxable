using System;

namespace Sandboxable.Hyak.Common.Internals
{
    /// <summary>
    /// Wrapper class that provides manual reference count functionality
    /// </summary>
    /// <typeparam name="T">Type to wrap around. Must be disposable.</typeparam>
    internal class DisposableReference<T> : IDisposable
        where T : class, IDisposable
    {
        private object _lock;

        public T Reference
        {
            get;
            private set;
        }

        public uint ReferenceCount
        {
            get;
            private set;
        }

        public DisposableReference(T reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            this.Reference = reference;
            this.ReferenceCount = 1;
        }

        public void AddReference()
        {
            lock (this._lock)
            {
                var referenceCount = this;
                referenceCount.ReferenceCount = referenceCount.ReferenceCount + 1;
            }
        }

        public void ReleaseReference()
        {
            lock (this._lock)
            {
                if ((int)this.ReferenceCount == 0)
                {
                    throw new ObjectDisposedException(typeof(T).FullName);
                }

                if ((int)--this.ReferenceCount != 0)
                {
                    return;
                }

                this.Reference.Dispose();
                this.Reference = default(T);
            }
        }

        void IDisposable.Dispose()
        {
            this.ReleaseReference();
        }
    }
}