using Fathcore.Infrastructure;
using Xunit;

namespace Fathcore
{
    public class EngineTest
    {
        [Fact]
        public void Engine_Is_Created_By_Default()
        {
            Singleton<IEngine>.Instance = null;

            Assert.Null(Singleton<IEngine>.Instance);
            Assert.Equal(Engine.Current, Singleton<IEngine>.Instance);
            Assert.NotNull(Singleton<IEngine>.Instance);
        }

        [Fact]
        public void Should_Create_An_Instance_By_Itself()
        {
            Singleton<IEngine>.Instance = null;

            var engine = Engine.Create();

            Assert.Equal(engine, BaseSingleton.AllSingletons[typeof(IEngine)]);
            Assert.Equal(engine, Engine.Current);
            Assert.Equal(Engine.Current, Singleton<IEngine>.Instance);
        }

        [Fact]
        public void Should_Replace_An_Instance()
        {
            Singleton<IEngine>.Instance = null;

            var engine = Engine.Create();
            var newEngine = new Infrastructure.Engine();

            Engine.Replace(newEngine);

            Assert.Equal(Engine.Current, newEngine);
        }
    }
}
