using System;
using Fathcore.EntityFramework.Audit;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for AuditHandler.
    /// </summary>
    public static class AuditHandlerServicesExtensions
    {
        /// <summary>
        /// Adds an <see cref="IAuditHandler"/> service with an implementation type specified in TImplementation
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAuditHandler<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IAuditHandler
        {
            services.AddScoped<IAuditHandler, TImplementation>().AsSelf();
            return services;
        }

        /// <summary>
        /// Adds an <see cref="IAuditHandler"/> service with an implementation type specified in implementationType
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAuditHandler(this IServiceCollection services, Type implementationType)
        {
            var serviceType = typeof(IAuditHandler);
            if (!serviceType.IsAssignableFrom(implementationType) || !implementationType.IsClass)
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IAuditHandler)}.");

            services.AddScoped(serviceType, implementationType).AsSelf();
            return services;
        }
    }
}
