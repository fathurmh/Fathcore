using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fathcore.Tests")]
namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Represents dependency resolver.
    /// </summary>
    internal sealed class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Resolve dependency.
        /// </summary>
        /// <param name="type">Type of resolved service.</param>
        /// <returns>Resolved service.</returns>
        public object Resolve(Type type)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return _serviceProvider.GetRequiredService(type);
            }
        }

        /// <summary>
        /// Resolve dependency.
        /// </summary>
        /// <typeparam name="T">Type of resolved service.</typeparam>
        /// <returns>Resolved service.</returns>
        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Resolve dependencies.
        /// </summary>
        /// <param name="type">Type of resolved service.</param>
        /// <returns>Collection of resolved services.</returns>
        public IEnumerable<object> ResolveAll(Type type)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return _serviceProvider.GetServices(type);
            }
        }

        /// <summary>
        /// Resolve dependencies.
        /// </summary>
        /// <typeparam name="T">Type of resolved services.</typeparam>
        /// <returns>Collection of resolved services.</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)ResolveAll(typeof(T));
        }

        /// <summary>
        /// Resolve unregistered service.
        /// </summary>
        /// <param name="type">Type of service.</param>
        /// <returns>Resolved service.</returns>
        public object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                try
                {
                    IEnumerable<object> parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Exception("Unknown dependency");
                        return service;
                    });

                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }
    }
}
