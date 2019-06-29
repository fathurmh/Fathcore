using System;
using Fathcore.Infrastructure.TypeFinders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for TypeFinder.
    /// </summary>
    public static class TypeFinderServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="ITypeFinder"/> service with default implementation type to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTypeFinder(this IServiceCollection services)
        {
            services.AddTypeFinder<TypeFinder>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITypeFinder"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTypeFinder<TImplementation>(this IServiceCollection services)
            where TImplementation : class, ITypeFinder
        {
            services.AddTypeFinder(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITypeFinder"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTypeFinder(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(ITypeFinder);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(ITypeFinder)}.");

            services.TryAddSingleton(serviceType, implementationType);

            return services;
        }
    }
}
