using Fathcore.MemoryCache;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for MemoryCache.
    /// </summary>
    public static class MemoryCacheServicesExtensions
    {
        /// <summary>
        /// Add memory cache service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMemoryCacheManager(this IServiceCollection services)
        {
            services.AddScoped<ICacheManager, ScopedCacheManager>();
            services.AddSingleton<MemoryCacheManager>();
            services.AddSingleton<ILocker>(provider => provider.GetRequiredService<MemoryCacheManager>());
            services.AddSingleton<IStaticCacheManager>(provider => provider.GetRequiredService<MemoryCacheManager>());

            return services;
        }
    }
}
