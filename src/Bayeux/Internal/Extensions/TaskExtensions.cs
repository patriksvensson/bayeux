using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal static class TaskExtensions
    {
        // http://stackoverflow.com/a/28626769
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            if (task.IsCompleted)
            {
                return task;
            }

            return task.ContinueWith(
                completedTask => completedTask.GetAwaiter().GetResult(),
                token,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
