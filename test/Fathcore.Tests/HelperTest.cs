using System;
using System.Linq;
using Fathcore.Infrastructure;
using Xunit;

namespace Fathcore.Tests
{
    public class HelperTest : TestBase
    {
        [Fact]
        public void Helper_IsCreated_ByDefault()
        {
            Singleton<IHelper>.Instance = null;

            Assert.Null(Singleton<IHelper>.Instance);
            Assert.Same(Helper.Current, Singleton<IHelper>.Instance);
            Assert.NotNull(Singleton<IHelper>.Instance);
        }

        [Fact]
        public void Helper_CreateAnInstance_ByItSelf()
        {
            var helper = Helper.Current;

            Assert.Same(helper, Singleton<IHelper>.Instance);
            Assert.Same(helper, BaseSingleton.AllSingletons[typeof(IHelper)]);
            Assert.Same(helper, Helper.Current);
            Assert.Same(Singleton<IHelper>.Instance, Helper.Current);
        }

        [Fact]
        public void Helper_ShouldReplace_AnInstance()
        {
            var typeFinder = new TypeFinder();
            var helper = Helper.Current;
            var newHelper = (IHelper)Activator.CreateInstance(typeFinder.FindClassesOfType<IHelper>().First());

            Assert.Same(helper, Helper.Current);
            Assert.NotSame(newHelper, Helper.Current);

            Helper.Replace(newHelper);

            Assert.Same(newHelper, Helper.Current);
        }
    }
}
