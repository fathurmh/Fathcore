using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fathcore.Infrastructure.DependencyInjection;
using Fathcore.Infrastructure.TypeFinders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fathcore.Infrastructure.Engines
{
    /// <summary>
    /// Represents an engine that can serve as a portal for the various services.
    /// </summary>
    internal sealed class Engine : IEngine
    {
        private IServiceProvider _serviceProvider;
        private ITypeFinder _typeFinder;

        private IHttpContextAccessor HttpContextAccessor => _serviceProvider.GetService<IHttpContextAccessor>();
        private IServiceProvider ServiceProvider => HttpContextAccessor?.HttpContext?.RequestServices ?? _serviceProvider;
        private ITypeFinder TypeFinder => ServiceProvider.GetService<ITypeFinder>() ?? _typeFinder;

        public Engine()
        {
            _serviceProvider = new ServiceCollection().BuildServiceProvider();
        }

        /// <summary>
        /// Populating service collection to DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate the service to.</param>
        /// <param name="configure">An <see cref="Action"/> to configure the provided <see cref="EngineOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IEngine Populate(IServiceCollection services, Action<EngineOptions> configure = default)
        {
            var options = new EngineOptions();
            configure?.Invoke(options);

            _typeFinder = options.TypeFinder;

            if (options.ActivateAttributeRegistrar)
            {
                var registrar = new DependencyAttributeRegistrar();
                var types = _typeFinder.FindClassesWithAttribute<RegisterServiceAttribute>();

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                    var interfaces = type.GetInterfaces();

                    if (interfaces.Any())
                        registrar.RegisterAsImplemented(services, type, interfaces, attribute);
                    else
                        registrar.RegisterAsSelf(services, type, attribute);
                }
            }

            if (options.ActivateClassRegistrar)
            {
                var dependencyRegistrarTypes = _typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var dependencyRegistrars = dependencyRegistrarTypes
                    .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar));

                foreach (var dependencyRegistrar in dependencyRegistrars)
                    dependencyRegistrar.Register(services);
            }

            services.TryAddSingleton(TypeFinder);
            services.TryAddSingleton(provider => (TypeFinder)provider.GetRequiredService<ITypeFinder>());
            services.TryAddSingleton(services);
            services.TryAddSingleton(provider => (ServiceCollection)provider.GetRequiredService<IServiceCollection>());

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
    }
}
