using System;
using Fathcore.MemoryCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for MemoryCache.
    /// </summary>
    public static class MemoryCacheServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddScopedMemoryCacheManager(this IServiceCollection services)
        {
            services.AddScopedMemoryCacheManager<ScopedCacheManager>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddScopedMemoryCacheManager<TImplementation>(this IServiceCollection services)
            where TImplementation : class, ICacheManager
        {
            services.AddScopedMemoryCacheManager(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddScopedMemoryCacheManager(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(ICacheManager);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(ICacheManager)}.");

            services.AddScoped(implementationType);
            services.AddScoped(serviceType, provider => provider.GetRequiredService(implementationType));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with default implementation type to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddScopedMemoryCacheManager(this IServiceCollection services)
        {
            services.TryAddScopedMemoryCacheManager<ICacheManager>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddScopedMemoryCacheManager<TImplementation>(this IServiceCollection services)
            where TImplementation : class, ICacheManager
        {
            services.TryAddScopedMemoryCacheManager(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICacheManager"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddScopedMemoryCacheManager(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(ICacheManager);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(ICacheManager)}.");

            services.TryAddScoped(implementationType);
            services.TryAddScoped(serviceType, provider => provider.GetRequiredService(implementationType));

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMemoryCacheManager(this IServiceCollection services)
        {
            services.AddMemoryCacheManager<MemoryCacheManager>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMemoryCacheManager<TImplementation>(this IServiceCollection services)
            where TImplementation : ILocker, IStaticCacheManager
        {
            services.AddMemoryCacheManager(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMemoryCacheManager(this IServiceCollection services, Type implementationType)
        {
            var serviceType = new[] { typeof(IStaticCacheManager), typeof(ILocker) };
            if (!(serviceType[0].IsAssignableFrom(implementationType) && serviceType[1].IsAssignableFrom(implementationType)) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IStaticCacheManager)} and {nameof(ILocker)}.");

            services.AddSingleton(implementationType);
            services.AddSingleton(serviceType[0], provider => provider.GetRequiredService(implementationType));
            services.AddSingleton(serviceType[1], provider => provider.GetRequiredService(implementationType));

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with default implementation type to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddMemoryCacheManager(this IServiceCollection services)
        {
            services.TryAddMemoryCacheManager<IStaticCacheManager>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with an implementation type specified in TImplementation to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddMemoryCacheManager<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IStaticCacheManager
        {
            services.TryAddMemoryCacheManager(typeof(TImplementation));

            return services;
        }

        /// <summary>
        /// Adds <see cref="IStaticCacheManager"/> and <see cref="ILocker"/> service with an implementation type specified in implementationType to the specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection TryAddMemoryCacheManager(this IServiceCollection services, Type implementationType)
        {
            var serviceType = new[] { typeof(IStaticCacheManager), typeof(ILocker) };
            if (!serviceType[0].IsAssignableFrom(implementationType) || !serviceType[1].IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IStaticCacheManager)} and {nameof(ILocker)}.");

            services.TryAddSingleton(implementationType);
            services.TryAddSingleton(serviceType[0], provider => provider.GetRequiredService(implementationType));
            services.TryAddSingleton(serviceType[1], provider => provider.GetRequiredService(implementationType));

            return services;
        }
    }
}
