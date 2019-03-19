using Fathcore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for EtagFactory.
    /// </summary>
    public static class EtagFactoryServicesExtensions
    {
        /// <summary>
        /// Add etag factory service.
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddEtagFactory(this IServiceCollection services)
        {
            services.AddScoped<EtagFactory>();
            services.AddScoped<IEtagFactory>(provider => provider.GetRequiredService<EtagFactory>());

            return services;
        }
    }
}
