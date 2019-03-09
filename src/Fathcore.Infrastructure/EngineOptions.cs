using System;
using Fathcore.Infrastructure.DependencyInjection;

namespace Fathcore.Infrastructure
{
    public class EngineOptions
    {
        internal Type DependencyResolverType { get; set; } = typeof(DependencyResolver);

        public void UseDependencyResolver<T>() where T : class, IDependencyResolver
        {
            UseDependencyResolver(typeof(T));
        }

        public void UseDependencyResolver(Type type)
        {
            if (!(typeof(IDependencyResolver).IsAssignableFrom(type) && type.IsClass))
                throw new InvalidOperationException($"Dependency resolver should implements from {nameof(IDependencyResolver)}.");

            DependencyResolverType = type;
        }
    }
}
