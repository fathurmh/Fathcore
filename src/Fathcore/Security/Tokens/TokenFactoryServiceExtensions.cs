using System;
using Fathcore.Security.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for TokenFactory.
    /// </summary>
    public static class TokenFactoryServiceExtensions
    {
        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTokenFactory(this IServiceCollection services)
        {
            services.AddTokenFactory<TokenFactory>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTokenFactory<TImplementation>(this IServiceCollection services)
            where TImplementation : ITokenFactory
        {
            services.AddTokenFactory(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddTokenFactory(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(ITokenFactory);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(ITokenFactory)}.");

            services.AddScoped(implementationType);
            services.AddScoped(serviceType, provider => provider.GetRequiredService(implementationType));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with default implementation type to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddTokenFactory(this IServiceCollection services)
        {
            services.TryAddTokenFactory<ITokenFactory>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddTokenFactory<TImplementation>(this IServiceCollection services)
            where TImplementation : class, ITokenFactory
        {
            services.TryAddTokenFactory(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ITokenFactory"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddTokenFactory(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(ITokenFactory);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(ITokenFactory)}.");

            services.TryAddScoped(implementationType);
            services.TryAddScoped(serviceType, provider => provider.GetRequiredService(implementationType));

            return services;
        }
    }
}
