using System;
using Fathcore.DependencyInjection;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Provides programmatic configuration for the engine.
    /// </summary>
    public class EngineOptions
    {
        internal Type DependencyResolverType { get; set; } = typeof(DependencyResolver);

        /// <summary>
        /// Configure which <see cref="IDependencyResolver"/> implementation to use.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the <see cref="IDependencyResolver"/> implementation to use.</typeparam>
        public void UseDependencyResolver<TImplementation>() where TImplementation : class, IDependencyResolver
        {
            UseDependencyResolver(typeof(TImplementation));
        }

        /// <summary>
        /// Configure which <see cref="IDependencyResolver"/> implementation to use.
        /// </summary>
        /// <param name="implementationType">The implementation type of the <see cref="IDependencyResolver"/>.</param>
        public void UseDependencyResolver(Type implementationType)
        {
            if (!(typeof(IDependencyResolver).IsAssignableFrom(implementationType) && implementationType.IsClass))
                throw new InvalidOperationException($"The {nameof(implementationType)} must be concrete class and implements {nameof(IDependencyResolver)}.");

            DependencyResolverType = implementationType;
        }
    }
}
