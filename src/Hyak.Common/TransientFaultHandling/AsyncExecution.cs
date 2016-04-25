using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Sandboxable.Hyak.Common.Properties;

namespace Sandboxable.Hyak.Common.TransientFaultHandling
{
    internal class AsyncExecution : AsyncExecution<bool>
    {
        private static Task<bool> cachedBoolTask;

        public AsyncExecution(Func<Task> taskAction, ShouldRetry shouldRetry, Func<Exception, bool> isTransient,
            Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken)
            : base(
                () => StartAsGenericTask(taskAction), shouldRetry, isTransient, onRetrying, fastFirstRetry,
                cancellationToken)
        {
        }

        private static Task<bool> GetCachedTask()
        {
            if (cachedBoolTask == null)
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.TrySetResult(true);
                cachedBoolTask = taskCompletionSource.Task;
            }
            return cachedBoolTask;
        }

        private static Task<bool> StartAsGenericTask(Func<Task> taskAction)
        {
            var task = taskAction();
            if (task == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TaskCannotBeNull, nameof(taskAction)), nameof(taskAction));
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                return GetCachedTask();
            }

            if (task.Status == TaskStatus.Created)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TaskMustBeScheduled, nameof(taskAction)), nameof(taskAction));
            }

            var tcs = new TaskCompletionSource<bool>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(true);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}