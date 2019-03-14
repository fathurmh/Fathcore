using System;
using Microsoft.Extensions.DependencyInjection;

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
            var firstDescriptor = new ServiceDescriptor(serviceTypes[0], implementationType, (ServiceLifetime)attribute.Lifetime);
            services.Add(firstDescriptor);

            for (int i = 1; i < serviceTypes.Length; i++)
            {
                services.Add(new ServiceDescriptor(serviceTypes[i], firstDescriptor.ImplementationFactory, (ServiceLifetime)attribute.Lifetime));
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
            services.Add(new ServiceDescriptor(implementationType, implementationType, (ServiceLifetime)attribute.Lifetime));

            return services;
        }
    }
}
