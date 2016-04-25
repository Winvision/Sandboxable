using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Sandboxable.Hyak.Common.Properties;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    internal class AsyncExecution<TResult>
    {
        private readonly Func<Task<TResult>> _taskFunc;

        private readonly ShouldRetry _shouldRetry;

        private readonly Func<Exception, bool> _isTransient;

        private readonly Action<int, Exception, TimeSpan> _onRetrying;

        private readonly bool _fastFirstRetry;

        private readonly CancellationToken _cancellationToken;

        private Task<TResult> _previousTask;

        private int _retryCount;

        public AsyncExecution(Func<Task<TResult>> taskFunc, ShouldRetry shouldRetry, Func<Exception, bool> isTransient, Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken)
        {
            this._taskFunc = taskFunc;
            this._shouldRetry = shouldRetry;
            this._isTransient = isTransient;
            this._onRetrying = onRetrying;
            this._fastFirstRetry = fastFirstRetry;
            this._cancellationToken = cancellationToken;
        }

        internal Task<TResult> ExecuteAsync()
        {
            return this.ExecuteAsyncImpl(null);
        }

        private Task<TResult> ExecuteAsyncContinueWith(Task<TResult> runningTask)
        {
            if (!runningTask.IsFaulted || this._cancellationToken.IsCancellationRequested)
            {
                return runningTask;
            }

            var zero = TimeSpan.Zero;

            var innerException = runningTask.Exception.InnerException;
            if (innerException is RetryLimitExceededException)
            {
                var taskCompletionSource = new TaskCompletionSource<TResult>();
                if (innerException.InnerException == null)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    taskCompletionSource.TrySetException(innerException.InnerException);
                }
                return taskCompletionSource.Task;
            }

            if (this._isTransient.Invoke(innerException))
            {
                var shouldRetry = this._shouldRetry;
                var asyncExecution = this;
                var num = asyncExecution._retryCount;
                var num1 = num;
                asyncExecution._retryCount = num + 1;

                if (shouldRetry(num1, innerException, out zero))
                {
                    if (zero < TimeSpan.Zero)
                    {
                        zero = TimeSpan.Zero;
                    }
                    this._onRetrying.Invoke(this._retryCount, innerException, zero);
                    this._previousTask = runningTask;
                    if (!(zero > TimeSpan.Zero) || this._retryCount <= 1 && this._fastFirstRetry)
                    {
                        return this.ExecuteAsyncImpl(null);
                    }

                    return Task.Delay(zero).ContinueWith(this.ExecuteAsyncImpl, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Unwrap();
                }
            }
            return runningTask;
        }

        private Task<TResult> ExecuteAsyncImpl(Task taskFunc)
        {
            Task<TResult> task;

            if (this._cancellationToken.IsCancellationRequested)
            {
                if (this._previousTask != null)
                {
                    return this._previousTask;
                }

                var completionSource = new TaskCompletionSource<TResult>();
                completionSource.TrySetCanceled();
                return completionSource.Task;
            }

            try
            {
                task = this._taskFunc();
            }
            catch (Exception exception)
            {
                if (this._isTransient(exception))
                {
                    var completionSource = new TaskCompletionSource<TResult>();
                    completionSource.TrySetException(exception);
                    task = completionSource.Task;
                }
                else
                {
                    throw;
                }
            }

            if (task == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TaskCannotBeNull, nameof(taskFunc)), nameof(taskFunc));
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                return task;
            }

            if (task.Status == TaskStatus.Created)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TaskMustBeScheduled, nameof(taskFunc)), nameof(taskFunc));
            }

            return task.ContinueWith(this.ExecuteAsyncContinueWith, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Unwrap();
        }
    }
}