using System.Threading;
using System.Threading.Tasks;

namespace Fathcore.Infrastructure.StartupTask
{
    /// <summary>
    /// Represents asynchronous startup task interface.
    /// </summary>
    public interface IAsyncStartupTask
    {
        /// <summary>
        /// Asynchronously start a startup task.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}
