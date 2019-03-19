using System;
using Fathcore.ResponseWrapper;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for AuditHandler.
    /// </summary>
    public static class ResponseWrapperServicesExtensions
    {
        /// <summary>
        /// Add response wrapper services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="configure">The <see cref="ResponseWrapperOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddResponseWrapper(this IServiceCollection services, Action<ResponseWrapperOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var options = new ResponseWrapperOptions();

            configure.Invoke(options);

            services.AddSingleton(typeof(IResponseHandler), options.ResponseHandler);

            return services;
        }
    }
}
