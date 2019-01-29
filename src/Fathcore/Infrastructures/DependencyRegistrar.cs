using Fathcore.Abstractions;
using Fathcore.Data.Abstractions;
using Fathcore.Infrastructures.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Fathcore.Infrastructures
{
    /// <summary>
    /// Represents dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register dependency using scrutor
        /// </summary>
        public DependencyRegistrar()
        {
            EngineContext.Current.ServiceCollection.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                    .AsMatchingInterface()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<IScopedService>())
                    .AsMatchingInterface()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo<ITransientService>())
                    .AsMatchingInterface()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IDbContext)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }
    }
}
