using System;
using System.Threading;

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
        /// <param name="action">An <see cref="Action"/> to be performed.</param>
        public void ExecuteIfNotCancelled(CancellationToken token, Action action)
        {
            token.ThrowIfCancellationRequested();
            action();
        }
    }
}
