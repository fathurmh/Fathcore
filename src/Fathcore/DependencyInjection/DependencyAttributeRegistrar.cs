using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Represents dependency attribute registrar.
    /// </summary>
    internal sealed class DependencyAttributeRegistrar : IDependencyAttributeRegistrar
    {
        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/> as interface implemented.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <param name="serviceTypes">The type of the service to register.</param>
        /// <param name="attribute">The <see cref="RegisterServiceAttribute"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IServiceCollection RegisterAsImplemented(IServiceCollection services, Type implementationType, Type[] serviceTypes, RegisterServiceAttribute attribute)
        {
            RegisterAsSelf(services, implementationType, attribute);
            foreach (var serviceType in serviceTypes)
            {
                services.TryAdd(new ServiceDescriptor(serviceType, provider => provider.GetRequiredService(implementationType), (ServiceLifetime)attribute.Lifetime));
            }

            return services;
        }

        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/> as self implemented.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <param name="attribute">The <see cref="RegisterServiceAttribute"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IServiceCollection RegisterAsSelf(IServiceCollection services, Type implementationType, RegisterServiceAttribute attribute)
        {
            services.TryAdd(new ServiceDescriptor(implementationType, implementationType, (ServiceLifetime)attribute.Lifetime));

            return services;
        }
    }
}
