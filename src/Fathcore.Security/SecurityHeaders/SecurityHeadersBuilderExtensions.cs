using System;
using Fathcore.Security.SecurityHeaders;
using Microsoft.AspNetCore.Builder;

namespace Fathcore.Extensions.Builder
{
    /// <summary>
    /// Extension methods for the SecurityHeaders middleware.
    /// </summary>
    public static class SecurityHeadersBuilderExtensions
    {
        /// <summary>
        /// Use security headers.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="action">Configure provided SecurityHeadersBuilder.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Action<SecurityHeadersFactory> action = default)
        {
            var builder = new SecurityHeadersFactory();

            if (action == null)
                builder.AddDefaultSecurePolicy();
            else
                action.Invoke(builder);

            return app.UseMiddleware<SecurityHeadersMiddleware>(builder.Build());
        }
    }
}
