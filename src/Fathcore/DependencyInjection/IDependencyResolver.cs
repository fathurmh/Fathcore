using System;
using System.Collections.Generic;

namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Represents dependency resolver interface.
    /// </summary>
    public interface IDependencyResolver
    {
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
