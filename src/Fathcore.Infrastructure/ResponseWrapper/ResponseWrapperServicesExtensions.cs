using System;
using Fathcore.Infrastructure.ResponseWrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for ResponseWrapper.
    /// </summary>
    public static class ResponseWrapperServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="IResponseHandler"/> service with default implementation type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="configure"></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddResponseWrapper(this IServiceCollection services, Action<ResponseWrapperOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var options = new ResponseWrapperOptions();

            configure.Invoke(options);

            services.TryAddSingleton(typeof(IResponseHandler), options.ResponseHandler);
            services.AddMvcCore(opt => opt.Filters.Add<ResponseWrapperFilter>());

            return services;
        }
    }
}
