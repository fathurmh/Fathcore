using System;
using System.Collections.Generic;
using System.Globalization;
using Fathcore.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Fathcore.Extensions.Builder
{
    /// <summary>
    /// Extension methods for the LocalizationProvider middleware.
    /// </summary>
    public static class LocalizationProviderBuilderExtensions
    {
        /// <summary>
        /// Configure app localization with <see cref="LocalizationProvider"/> provider.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="configure">The <see cref="RequestLocalizationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app, Action<RequestLocalizationOptions> configure = default)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("id-ID"),
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            localizationOptions.RequestCultureProviders.Insert(0, new LocalizationProvider());
            configure?.Invoke(localizationOptions);
            app.UseRequestLocalization(localizationOptions);

            return app;
        }
    }
}
