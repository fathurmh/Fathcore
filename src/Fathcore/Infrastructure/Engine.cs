using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fathcore.DependencyInjection;
using Fathcore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Represents an engine that can serve as a portal for the various services.
    /// </summary>
    public sealed class Engine : IEngine
    {
        private IServiceProvider _serviceProvider;
        private IHttpContextAccessor HttpContextAccessor => _serviceProvider.GetService<IHttpContextAccessor>();
        private IServiceProvider ServiceProvider => HttpContextAccessor?.HttpContext?.RequestServices ?? _serviceProvider;

        /// <summary>
        /// Populating service collection to DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IEngine Populate(IServiceCollection services)
        {
            ActivateDependencyRegistrar(services);
            ActivateAttributeRegistrar(services);

            services.AddSingleton(typeof(IServiceCollection), services).AsSelf();

            _serviceProvider = services.BuildServiceProvider();

            return this;
        }

        /// <summary>
        /// Resolve dependency.
        /// </summary>
        /// <param name="type">Type of resolved service.</param>
        /// <returns>Resolved service.</returns>
        public object Resolve(Type type)
        {
            return ServiceProvider.GetService(type);
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
            return ServiceProvider.GetServices(type);
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

        /// <summary>
        /// Activate dependency registrar for adding services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        private IEngine ActivateDependencyRegistrar(IServiceCollection services)
        {
            var typeFinder = new TypeFinder();
            var dependencyRegistrarTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var dependencyRegistrars = dependencyRegistrarTypes
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar));

            foreach (var dependencyRegistrar in dependencyRegistrars)
                dependencyRegistrar.Register(services);

            return this;
        }

        /// <summary>
        /// Activate attributed dependency registrar for adding services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        private IEngine ActivateAttributeRegistrar(IServiceCollection services)
        {
            var typeFinder = new TypeFinder();
            var registrar = new DependencyAttributeRegistrar();

            var types = typeFinder.FindClassesWithAttribute<RegisterServiceAttribute>();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                var interfaces = type.GetInterfaces();

                if (!interfaces.Any())
                {
                    registrar.RegisterAsSelf(services, type, attribute);
                }
                else
                {
                    registrar.RegisterAsImplemented(services, type, interfaces, attribute);
                }
            }

            return this;
        }
    }
}
