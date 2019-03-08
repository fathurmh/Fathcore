using System.Linq;
using Fathcore.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class EngineTest
    {
        [Fact]
        public void Should_Populate_Service_Collection()
        {
            var services = new ServiceCollection();
            var engine = new Engine();

            engine.Populate(services);

            Assert.True(services.Count >= 3);
            Assert.Contains(services, prop => prop.ServiceType.FullName.Contains(nameof(IDependencyRegistrar)));
            Assert.Equal(ServiceLifetime.Singleton,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(DependencyRegistrarSingletonTest)).Lifetime);
            Assert.Equal(ServiceLifetime.Scoped,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(DependencyRegistrarScopedTest)).Lifetime);
            Assert.Equal(ServiceLifetime.Transient,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(DependencyRegistrarTransientTest)).Lifetime);
        }

        class DependencyRegistrarSingletonTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddSingleton<IDependencyRegistrar, DependencyRegistrarSingletonTest>();
            }
        }

        class DependencyRegistrarScopedTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddScoped<IDependencyRegistrar, DependencyRegistrarScopedTest>();
            }
        }

        class DependencyRegistrarTransientTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddTransient<IDependencyRegistrar, DependencyRegistrarTransientTest>();
            }
        }
    }
}
