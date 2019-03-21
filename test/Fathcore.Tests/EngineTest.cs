using Fathcore.Infrastructure;
using Xunit;

namespace Fathcore.Tests
{
    public class EngineTest : TestBase
    {
        [Fact]
        public void Engine_IsCreated_ByDefault()
        {
            Singleton<IEngine>.Instance = null;

            Assert.Null(Singleton<IEngine>.Instance);
            Assert.Same(Engine.Current, Singleton<IEngine>.Instance);
            Assert.NotNull(Singleton<IEngine>.Instance);
        }

        [Fact]
        public void Engine_CreateAnInstance_ByItSelf()
        {
            var engine = Engine.Create();

            Assert.Same(engine, BaseSingleton.AllSingletons[typeof(IEngine)]);
            Assert.Same(engine, Engine.Current);
            Assert.Same(Singleton<IEngine>.Instance, Engine.Current);
        }

        [Fact]
        public void Engine_ShouldReplace_AnInstance()
        {
            var engine = Engine.Create();
            var newEngine = new Infrastructure.Engine();

            Assert.Same(engine, Engine.Current);
            Assert.NotSame(newEngine, Engine.Current);

            Engine.Replace(newEngine);

            Assert.Same(newEngine, Engine.Current);
        }
    }
}
