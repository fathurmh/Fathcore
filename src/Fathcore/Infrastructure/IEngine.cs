using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the engine. 
    /// Edit functionality, modules and implementations access most functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Populating service collection to DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate the service to.</param>
        /// <param name="action">An <see cref="Action"/> to configure the provided <see cref="EngineOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        IEngine Populate(IServiceCollection services, Action<EngineOptions> action = default);

        /// <summary>
        /// Resolve dependency.
        /// </summary>
        /// <param name="type">Type of resolved service.</param>
        /// <returns>Resolved service.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependency.
        /// </summary>
        /// <typeparam name="T">Type of resolved service.</typeparam>
        /// <returns>Resolved service.</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve dependencies.
        /// </summary>
        /// <param name="type">Type of resolved service.</param>
        /// <returns>Collection of resolved services.</returns>
        IEnumerable<object> ResolveAll(Type type);

        /// <summary>
        /// Resolve dependencies.
        /// </summary>
        /// <typeparam name="T">Type of resolved services.</typeparam>
        /// <returns>Collection of resolved services.</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Resolve unregistered service.
        /// </summary>
        /// <param name="type">Type of service.</param>
        /// <returns>Resolved service.</returns>
        object ResolveUnregistered(Type type);
    }
}
