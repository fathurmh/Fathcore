using System;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Provides the interface for dependency attribute registrar that can register services.
    /// </summary>
    internal interface IDependencyAttributeRegistrar
    {
        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/> as self implemented.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <param name="attribute">The <see cref="RegisterServiceAttribute"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        IServiceCollection RegisterAsSelf(IServiceCollection services, Type implementationType, RegisterServiceAttribute attribute);

        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/> as interface implemented.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <param name="serviceTypes">The type of the service to register.</param>
        /// <param name="attribute">The <see cref="RegisterServiceAttribute"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        IServiceCollection RegisterAsImplemented(IServiceCollection services, Type implementationType, Type[] serviceTypes, RegisterServiceAttribute attribute);
    }
}
