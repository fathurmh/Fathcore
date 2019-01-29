using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fathcore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if file changes (default is false)</param>
        /// <returns>Instance of configuration parameters</returns>
        public static IServiceCollection AddStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration, bool reloadOnChange = false) where TConfig : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();

            if (reloadOnChange)
            {
                services.Configure<TConfig>(configuration);
                services.AddScoped(option => option.GetService<IOptionsSnapshot<TConfig>>().Value);
            }
            else
            {
                configuration.Bind(config);
                services.AddSingleton(config);
            }

            return services;
        }
    }
}
