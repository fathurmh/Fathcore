using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure.Localization
{
    /// <summary>
    /// Represents default localization provider.
    /// </summary>
    public class LocalizationProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var cultureInfo = httpContext.RequestServices.GetService<ICultureInfo>();
            string defaultCulture = cultureInfo.Default;
            string defaultUICulture = cultureInfo.DefaultUI;

            if (defaultCulture == null && defaultUICulture == null)
                return Task.FromResult((ProviderCultureResult)null);

            if (defaultCulture != null && defaultUICulture == null)
                defaultUICulture = defaultCulture;

            if (defaultCulture == null && defaultUICulture != null)
                defaultCulture = defaultUICulture;

            var requestCulture = new ProviderCultureResult(defaultCulture, defaultUICulture);

            return Task.FromResult(requestCulture);
        }
    }
}
