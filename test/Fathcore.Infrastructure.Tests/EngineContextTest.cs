using System;
using System.Linq;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class EngineContextTest : IDisposable
    {
        [Fact]
        public void Engine_IsCreated_ByDefault()
        {
            Singleton<IEngine>.Instance = null;

            Assert.Null(Singleton<IEngine>.Instance);
            Assert.Same(EngineContext.Current, Singleton<IEngine>.Instance);
            Assert.NotNull(Singleton<IEngine>.Instance);
        }

        [Fact]
        public void Engine_CreateAnInstance_ByItSelf()
        {
            var engine = EngineContext.Create();

            Assert.Same(engine, Singleton<IEngine>.Instance);
            Assert.Same(engine, BaseSingleton.AllSingletons[typeof(IEngine)]);
            Assert.Same(engine, EngineContext.Current);
            Assert.Same(Singleton<IEngine>.Instance, EngineContext.Current);
        }

        [Fact]
        public void Engine_ShouldReplace_AnInstance()
        {
            var typeFinder = new TypeFinder();
            var engine = EngineContext.Create();
            var newEngine = (IEngine)Activator.CreateInstance(typeFinder.FindClassesOfType<IEngine>().First());

            Assert.Same(engine, EngineContext.Current);
            Assert.NotSame(newEngine, EngineContext.Current);

            EngineContext.Replace(newEngine);

            Assert.Same(newEngine, EngineContext.Current);
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
