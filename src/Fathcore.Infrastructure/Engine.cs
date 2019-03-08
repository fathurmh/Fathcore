using System;
using System.Linq;
using Fathcore.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Represents an engine that can serve as a portal for the various services.
    /// </summary>
    public class Engine : IEngine
    {
        private ServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected virtual IHttpContextAccessor HttpContextAccessor
            => _serviceProvider.GetService<IHttpContextAccessor>() ?? _httpContextAccessor;
        protected virtual IServiceProvider ServiceProvider
            => HttpContextAccessor?.HttpContext?.RequestServices ?? _serviceProvider;

        public Engine()
        {
            _serviceProvider = new ServiceCollection().BuildServiceProvider();
            _httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        }

        /// <summary>
        /// Populating service collection to DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public virtual IEngine Populate(IServiceCollection services)
        {
            ActivateDependencyRegistrar(services);

            services.AddSingleton(typeof(IServiceCollection), services);
            _serviceProvider = services.BuildServiceProvider();

            return this;
        }

        /// <summary>
        /// Activate dependency registrar for adding services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        protected virtual IEngine ActivateDependencyRegistrar(IServiceCollection services)
        {
            var typeFinder = new TypeFinder();
            var dependencyRegistrarTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var dependencyRegistrars = dependencyRegistrarTypes
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar));

            foreach (var dependencyRegistrar in dependencyRegistrars)
                dependencyRegistrar.Register(services);

            return this;
        }
    }
}
