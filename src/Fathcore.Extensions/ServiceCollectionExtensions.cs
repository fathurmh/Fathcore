using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the last registration as its own type.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AsSelf(this IServiceCollection services)
        {
            var lastRegistration = services.LastOrDefault();
            if (lastRegistration != null)
            {
                var implementationType = GetImplementationType(lastRegistration);

                if (lastRegistration.ServiceType == implementationType)
                    return services;

                if (lastRegistration.ImplementationInstance != null)
                {
                    services.Add(new ServiceDescriptor(implementationType, lastRegistration.ImplementationInstance));
                }
                else
                {
                    services.Remove(lastRegistration);

                    if (lastRegistration.ImplementationFactory != null)
                    {
                        services.Add(new ServiceDescriptor(
                            implementationType,
                            lastRegistration.ImplementationFactory,
                            lastRegistration.Lifetime));
                    }
                    else
                    {
                        services.Add(new ServiceDescriptor(
                            implementationType,
                            implementationType,
                            lastRegistration.Lifetime));
                    }

                    services.Add(new ServiceDescriptor(
                        lastRegistration.ServiceType,
                        provider => provider.GetService(implementationType),
                        lastRegistration.Lifetime));
                }
            }

            return services;
        }

        private static Type GetImplementationType(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                return descriptor.ImplementationType;

            if (descriptor.ImplementationInstance != null)
                return descriptor.ImplementationInstance.GetType();

            if (descriptor.ImplementationFactory != null)
                return descriptor.ImplementationFactory.GetType().GenericTypeArguments[1];

            return null;
        }
    }
}
