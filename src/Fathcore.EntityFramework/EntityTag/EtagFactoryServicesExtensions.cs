using System;
using Fathcore.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for EtagFactory.
    /// </summary>
    public static class EtagFactoryServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="IEtagFactory"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddEtagFactory(this IServiceCollection services)
        {
            services.AddEtagFactory<EtagFactory>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IEtagFactory"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddEtagFactory<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IEtagFactory
        {
            services.AddEtagFactory(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IEtagFactory"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddEtagFactory(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(IEtagFactory);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IEtagFactory)}.");

            services.AddHttpContextAccessor();
            services.AddHashFactory();
            services.TryAddScoped(serviceType, implementationType);

            return services;
        }
    }
}
