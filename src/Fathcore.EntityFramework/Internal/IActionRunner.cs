using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fathcore.EntityFramework.Internal
{
    /// <summary>
    /// Provides the interface for action runner.
    /// </summary>
    public interface IActionRunner
    {
        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Action"/> to be performed.</param>
        void ExecuteIfNotCancelled(CancellationToken token, Action acquire);

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        T ExecuteIfNotCancelled<T>(CancellationToken token, Func<T> acquire);

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        /// <returns></returns>
        Task ExecuteIfNotCancelledAsync(CancellationToken token, Func<Task> acquire);

        /// <summary>
        /// Executes an action if cancellation not requested.
        /// </summary>
        /// <typeparam name="T">Type of action result.</typeparam>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="acquire">An <see cref="Func{Task}"/> to be performed.</param>
        /// <returns></returns>
        Task<T> ExecuteIfNotCancelledAsync<T>(CancellationToken token, Func<Task<T>> acquire);
    }
}
