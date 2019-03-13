using System.Linq;
using Fathcore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Infrastructure
{
    public class EngineTest
    {
        [Fact]
        public void Should_Populate_Service_Collection_Using_DependencyRegistrar()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

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
        [Fact]
        public void Should_Populate_Service_Collection_Using_Attribute()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);

            Assert.True(services.Count >= 3);
            Assert.Equal(ServiceLifetime.Singleton,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(AttributeSingletonTest)).Lifetime);
            Assert.Equal(ServiceLifetime.Scoped,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(AttributeScopedTest)).Lifetime);
            Assert.Equal(ServiceLifetime.Transient,
                services.FirstOrDefault(prop => prop.ImplementationType == typeof(AttributeTransientTest)).Lifetime);
        }

        [Fact]
        public void Can_Resolve_Service()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instance = engine.Resolve<IServiceCollection>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Can_Resolve_Service_Implementation()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instance = engine.Resolve<ServiceCollection>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Can_Resolve_Service_2()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instance = engine.Resolve(typeof(ServiceCollection));

            Assert.NotNull(instance);
        }

        [Fact]
        public void Can_Resolve_Service_Implementation_2()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instance = engine.Resolve(typeof(ServiceCollection));

            Assert.NotNull(instance);
        }

        [Fact]
        public void Can_Resolve_All_Services()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instances = engine.ResolveAll<IServiceCollection>();

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);
        }

        [Fact]
        public void Can_Resolve_All_Service_Implementations()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instances = engine.ResolveAll<ServiceCollection>();

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);
        }

        [Fact]
        public void Can_Resolve_All_Services_2()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instances = engine.ResolveAll(typeof(IServiceCollection));

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);
        }

        [Fact]
        public void Can_Resolve_All_Service_Implementations_2()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instances = engine.ResolveAll(typeof(ServiceCollection));

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);
        }

        [Fact]
        public void Can_Resolve_Unregistered()
        {
            var services = new ServiceCollection();
            var engine = new Fathcore.Infrastructure.Engine();

            engine.Populate(services);
            var instance = engine.ResolveUnregistered(typeof(ServiceCollection));

            Assert.NotNull(instance);
        }

        private class DependencyRegistrarSingletonTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddSingleton<IDependencyRegistrar, DependencyRegistrarSingletonTest>();
            }
        }

        private class DependencyRegistrarScopedTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddScoped<IDependencyRegistrar, DependencyRegistrarScopedTest>();
            }
        }

        private class DependencyRegistrarTransientTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
            {
                return services.AddTransient<IDependencyRegistrar, DependencyRegistrarTransientTest>();
            }
        }

        [RegisterService(Lifetime.Singleton)]
        private class AttributeSingletonTest
        {
        }

        [RegisterService(Lifetime.Scoped)]
        private class AttributeScopedTest
        {
        }

        [RegisterService(Lifetime.Transient)]
        private class AttributeTransientTest
        {
        }
    }
}
