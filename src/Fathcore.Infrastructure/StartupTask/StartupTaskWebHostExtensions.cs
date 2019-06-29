using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.Infrastructure;
using Fathcore.Infrastructure.StartupTask;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fathcore.Extensions.Hosting
{
    /// <summary>
    /// Represents startup task web host extensions.
    /// </summary>
    public static class StartupTaskWebHostExtensions
    {
        /// <summary>
        /// Asynchronously runs a web application include startup tasks and returns a Task that only completes when the token is triggered or shutdown is triggered.
        /// </summary>
        /// <param name="webHost">The <see cref="IWebHost"/> to run.</param>
        /// <param name="cancellationToken">The token to trigger shutdown.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task RunWithTasksAsync(this IWebHost webHost, CancellationToken cancellationToken = default)
        {
            var logger = webHost.Services.GetService<ILogger<IWebHost>>() ?? new NullLogger<IWebHost>();

            var startupTasksAsync = webHost.Services.GetServices<IAsyncStartupTask>();
            var startupTasks = webHost.Services.GetServices<IStartupTask>();

            logger.LogInformation("Startup tasks execution started.");
            await startupTasksAsync.ForEachAsync(task => task.StartAsync(cancellationToken));
            startupTasks.OrderBy(task => task.Order).ToList().ForEach(task => task.Start());
            logger.LogInformation("Startup tasks execution finished.");

            await webHost.RunAsync(cancellationToken);
        }

        /// <summary>
        /// Runs a web application and block the calling thread until host shutdown.
        /// </summary>
        /// <param name="webHost">The <see cref="IWebHost"/> to run.</param>
        public static void RunWithTasks(this IWebHost webHost)
        {
            var logger = webHost.Services.GetService<ILogger<IWebHost>>() ?? new NullLogger<IWebHost>();

            var startupTasksAsync = webHost.Services.GetServices<IAsyncStartupTask>();
            var startupTasks = webHost.Services.GetServices<IStartupTask>();

            logger.LogInformation("Startup tasks execution started.");
            HelperContext.Current.RunSync(() => startupTasksAsync.ForEachAsync(task => task.StartAsync()));
            startupTasks.OrderBy(task => task.Order).ToList().ForEach(task => task.Start());
            logger.LogInformation("Startup tasks execution finished.");

            webHost.Run();
        }
    }
}
