using System;
using System.Threading;

namespace Fathcore.EntityFramework.Internal
{
    /// <summary>
    /// Provides the interface for action runner.
    /// </summary>
    public interface IActionRunner
    {
        /// <summary>
        /// Executes an action if cancel doesn't requested.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="action">An <see cref="Action"/> to be performed.</param>
        void ExecuteIfNotCancelled(CancellationToken token, Action action);
    }
}
