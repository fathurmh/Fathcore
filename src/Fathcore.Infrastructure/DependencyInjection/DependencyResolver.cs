using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fathcore.Infrastructure.Tests")]
namespace Fathcore.Infrastructure.DependencyInjection
{
    internal partial class DependencyResolver : IDependencyResolver
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
        public virtual object Resolve(Type type)
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
        public virtual T Resolve<T>() where T : class
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return (T)_serviceProvider.GetRequiredService(typeof(T));
            }
        }

        /// <summary>
        /// Resolve dependencies.
        /// </summary>
        /// <typeparam name="T">Type of resolved services.</typeparam>
        /// <returns>Collection of resolved services.</returns>
        public virtual IEnumerable<T> ResolveAll<T>()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return (IEnumerable<T>)_serviceProvider.GetServices(typeof(T));
            }
        }

        /// <summary>
        /// Resolve unregistered service.
        /// </summary>
        /// <param name="type">Type of service.</param>
        /// <returns>Resolved service.</returns>
        public virtual object ResolveUnregistered(Type type)
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
