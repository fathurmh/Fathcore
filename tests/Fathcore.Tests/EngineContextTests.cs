using System;
using System.Collections.Generic;
using System.Linq;
using Fathcore.Abstractions;
using Fathcore.Infrastructures;
using Fathcore.Infrastructures.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Tests
{
    public class EngineContextTests
    {   
        [Fact]
        public void Engine_Is_Created_By_Default()
        {
            Assert.Equal(EngineContext.Current, Singleton<IEngine>.Instance);
        }

        [Fact]
        public void Should_Create_An_Instance_By_Itself()
        {
            var engine = EngineContext.Create();

            Assert.Equal(engine, BaseSingleton.AllSingletons[typeof(IEngine)]);
            Assert.Equal(engine, EngineContext.Current);
            Assert.Equal(EngineContext.Current, Singleton<IEngine>.Instance);
        }
        
        [Fact]
        public void Should_Replace_An_Instance()
        {
            var engine = EngineContext.Create();
            engine.ConfigureServices();
            var newEngine = new CoreEngine();

            EngineContext.Replace(newEngine);
            
            Assert.Equal(EngineContext.Current, newEngine);
            Assert.Equal(EngineContext.Current.ServiceCollection.Count, newEngine.ServiceCollection.Count);
            Assert.NotEqual(EngineContext.Current.ServiceCollection.Count, engine.ServiceCollection.Count);
        }

        [Fact]
        public void Should_Register_Services_Dependencies()
        {
            var engine = EngineContext.Create();
            engine.ConfigureServices();
            var registeredServices = EngineContext.Current.ServiceCollection
                .Where(prop => prop.ServiceType.FullName.Contains(typeof(IDependencyRegistrar).Namespace.Split('.')[0]))
                .ToList();

            Assert.Contains(registeredServices, prop => prop.ServiceType.FullName.Contains(nameof(IDependencyRegistrar)));
        }
    }
}
