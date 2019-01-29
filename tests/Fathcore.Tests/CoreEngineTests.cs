using System;
using System.Linq;
using System.Net.Http;
using Fathcore.Infrastructures;
using Fathcore.Infrastructures.Abstractions;
using Fathcore.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Tests
{
    public class CoreEngineTests
    {
        private IServiceCollection Collection { get; } = new ServiceCollection();

        [Fact]
        public void Should_Register_Service_Dependency()
        {
            Collection.AddSingleton(typeof(ITransientService), typeof(TransientService));
            var engine = new CoreEngine(Collection);
            engine.ConfigureServices();

            using (var serviceScope = engine.ServiceProvider.CreateScope())
            {
                var requiredService = serviceScope.ServiceProvider.GetService<ITransientService>();

                Assert.NotNull(requiredService);
            }
        }

        [Fact]
        public void Should_Resolve_Service_Dependency()
        {
            Collection.AddSingleton(typeof(ITransientService), typeof(TransientService));
            var engine = new CoreEngine(Collection);
            engine.ConfigureServices();

            var requiredService = engine.Resolve<ITransientService>();

            Assert.NotNull(requiredService);
        }

        [Fact]
        public void Should_Not_Resolve_Service_Dependency()
        {
            var engine = new CoreEngine(Collection);
            engine.ConfigureServices();

            Assert.Throws<InvalidOperationException>(() => engine.Resolve<ITransientService>());
        }

        [Fact]
        public void Should_Resolve_All_Service_Dependency()
        {
            Collection.AddSingleton(typeof(ITransientService), typeof(TransientService));
            Collection.AddSingleton(typeof(ITransientService), typeof(TransientService1));
            var engine = new CoreEngine(Collection);
            engine.ConfigureServices();

            var requiredService = engine.ResolveAll<ITransientService>();

            Assert.Equal(2, requiredService.Count());
        }

        [Fact]
        public void Should_Resolve_Unregistered_Service_Dependency()
        {
            var engine = new CoreEngine(Collection);
            engine.ConfigureServices();

            var requiredService = engine.ResolveUnregistered(typeof(UnregisteredService));

            Assert.NotNull(requiredService);
        }

        [Fact]
        public void Engine_Http_Client_Is_Singleton()
        {
            var engine = new CoreEngine();
            
            HttpClient httpClient = engine.HttpClient;

            Assert.Equal(httpClient, Singleton<HttpClient>.Instance);
            Assert.Equal(httpClient, BaseSingleton.AllSingletons[typeof(HttpClient)]);
        }
    }
}
