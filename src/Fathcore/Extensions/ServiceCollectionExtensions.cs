using System;
using System.Collections.Generic;
using System.Globalization;
using Fathcore.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
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

        /// <summary>
        /// An ordered list of providers used to determine a request's culture information.
        /// The first provider that returns a non-null result for a given request will be used.
        /// Defaults to the following: 
        /// 
        /// <see cref="DefaultLocalizationProvider" />
        /// <see cref="QueryStringRequestCultureProvider" />
        /// <see cref="CookieRequestCultureProvider" />
        /// <see cref="AcceptLanguageHeaderRequestCultureProvider" />
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configure"></param>
        public static void ConfigureAppLocalization(this IApplicationBuilder app, Action<RequestLocalizationOptions> configure = null)
        {
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("id-ID"),
            };

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            localizationOptions.RequestCultureProviders.Insert(0, new DefaultLocalizationProvider());
            
            if (configure != null) configure(localizationOptions);

            app.UseRequestLocalization(localizationOptions);
        }
    }
}
