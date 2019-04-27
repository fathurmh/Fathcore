using System;
using Fathcore.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for Repository.
    /// </summary>
    public static class RepositoryServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="Repository{TEntity}"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddGenericRepository(this IServiceCollection services)
        {
            services.AddGenericRepository(typeof(Repository<>));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="Repository{TEntity}"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddGenericRepository(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(IRepository<>);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {typeof(IRepository<>).Name}.");

            services.TryAddSingleton(implementationType);
            services.TryAddSingleton(serviceType, provider => provider.GetRequiredService(implementationType));

            return services;
        }
    }
}
