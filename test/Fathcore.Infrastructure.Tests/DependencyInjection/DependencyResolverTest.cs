using System.Linq;
using Fathcore.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Infrastructure.Tests.DependencyInjection
{
    public class DependencyResolverTest
    {
        [Fact]
        public void Can_Resolve_Service()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<DependencyResolver>()
                .AddSingleton(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var resolver = scope.ServiceProvider.GetRequiredService<DependencyResolver>();
                var instance = resolver.Resolve(typeof(ServiceCollection));

                Assert.NotNull(instance);
            }
        }

        [Fact]
        public void Can_Resolve_Service_2()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<DependencyResolver>()
                .AddSingleton(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var resolver = scope.ServiceProvider.GetRequiredService<DependencyResolver>();
                var instance = resolver.Resolve<ServiceCollection>();

                Assert.NotNull(instance);
            }
        }

        [Fact]
        public void Can_Resolve_All_Services()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<DependencyResolver>()
                .AddSingleton(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var resolver = scope.ServiceProvider.GetRequiredService<DependencyResolver>();
                var instance = resolver.ResolveAll<ServiceCollection>();

                Assert.NotNull(instance);
                Assert.True(instance.Count() > 0);
            }
        }

        [Fact]
        public void Can_Resolve_Unregistered()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IDependencyResolver, DependencyResolver>()
                .AddSingleton<DependencyResolver>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var resolver = scope.ServiceProvider.GetRequiredService<DependencyResolver>();
                var instance = resolver.ResolveUnregistered(typeof(ServiceCollection));

                Assert.NotNull(instance);
            }
        }
    }
}
