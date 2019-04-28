using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fathcore.EntityFramework.Internal
{
    /// <summary>
    /// Provides the class for action runner.
    /// </summary>
    public class ActionRunner : IActionRunner
    {
        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Action"/> to be performed.</param>
        public void ExecuteIfNotCancelled(CancellationToken token, Action acquire)
        {
            token.ThrowIfCancellationRequested();
            acquire();
        }

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        public T ExecuteIfNotCancelled<T>(CancellationToken token, Func<T> acquire)
        {
            token.ThrowIfCancellationRequested();
            return acquire();
        }

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        /// <returns></returns>
        public async Task ExecuteIfNotCancelledAsync(CancellationToken token, Func<Task> acquire)
        {
            token.ThrowIfCancellationRequested();
            await acquire();
        }

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <typeparam name="T">Type of action result.</typeparam>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        /// <returns></returns>
        public async Task<T> ExecuteIfNotCancelledAsync<T>(CancellationToken token, Func<Task<T>> acquire)
        {
            token.ThrowIfCancellationRequested();
            return await acquire();
        }
    }
}
