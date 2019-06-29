using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fathcore.Infrastructure.StartupTask
{
    /// <summary>
    /// Represents warmup services startup task.
    /// </summary>
    public class WarmupServicesStartupTask : IAsyncStartupTask
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WarmupServicesStartupTask> _logger;

        public WarmupServicesStartupTask(IServiceCollection services, IServiceProvider serviceProvider, ILogger<WarmupServicesStartupTask> logger)
        {
            _services = services;
            _serviceProvider = serviceProvider;
            _logger = logger;

        }

        /// <summary>
        /// Asynchronously execute a startup task.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var service in GetServices(_services))
                {
                    _logger.LogInformation($"Service type {service.Name} starting to warm up.");

                    var result = await Task.Run(() => scope.ServiceProvider.GetService(service));

                    if (result != null)
                    {
                        _logger.LogInformation($"Service type {service.Name} was successfully warmed up.");
                    }
                    else
                    {
                        _logger.LogWarning($"Service type {service.Name} was failed to warmed up.");
                    }
                }
            }
        }

        private IEnumerable<Type> GetServices(IServiceCollection services)
        {
            return services
               .Where(descriptor => descriptor.ImplementationType != typeof(WarmupServicesStartupTask))
               .Where(descriptor => descriptor.ServiceType.ContainsGenericParameters == false)
               .Select(descriptor => descriptor.ServiceType)
               .Distinct();
        }
    }
}
