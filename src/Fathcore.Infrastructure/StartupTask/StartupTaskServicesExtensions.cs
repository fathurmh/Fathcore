using System;
using Fathcore.Infrastructure.StartupTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for StartupTask.
    /// </summary>
    public static class StartupTaskServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="IStartupTask"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddStartupTask<TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            services.AddStartupTask(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IStartupTask"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Type implementationType)
        {
            var serviceType = new[] { typeof(IStartupTask), typeof(IAsyncStartupTask) };
            if (!(serviceType[0].IsAssignableFrom(implementationType) || serviceType[1].IsAssignableFrom(implementationType)) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IStartupTask)} or {nameof(IAsyncStartupTask)}.");

            if (serviceType[0].IsAssignableFrom(implementationType))
            {
                services.AddSingleton(implementationType);
                services.AddSingleton(serviceType[0], provider => provider.GetRequiredService(implementationType));
            }

            if (serviceType[1].IsAssignableFrom(implementationType))
            {
                services.AddSingleton(implementationType);
                services.AddSingleton(serviceType[1], provider => provider.GetRequiredService(implementationType));
            }

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IStartupTask"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddStartupTask<TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            services.TryAddStartupTask(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IStartupTask"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddStartupTask(this IServiceCollection services, Type implementationType)
        {
            var serviceType = new[] { typeof(IStartupTask), typeof(IAsyncStartupTask) };
            if (!(serviceType[0].IsAssignableFrom(implementationType) || serviceType[1].IsAssignableFrom(implementationType)) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IStartupTask)} or {nameof(IAsyncStartupTask)}.");

            if (serviceType[0].IsAssignableFrom(implementationType))
            {
                services.TryAddSingleton(implementationType);
                services.TryAddSingleton(serviceType[0], provider => provider.GetRequiredService(implementationType));
            }

            if (serviceType[1].IsAssignableFrom(implementationType))
            {
                services.TryAddSingleton(implementationType);
                services.TryAddSingleton(serviceType[1], provider => provider.GetRequiredService(implementationType));
            }

            return services;
        }
    }
}
