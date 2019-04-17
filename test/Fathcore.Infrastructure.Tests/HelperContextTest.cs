using System;
using System.Linq;
using Fathcore.Infrastructure.Helpers;
using Fathcore.Infrastructure.TypeFinders;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class HelperContextTest
    {
        [Fact]
        public void Helper_IsCreated_ByDefault()
        {
            Singleton<IHelper>.Instance = null;

            Assert.Null(Singleton<IHelper>.Instance);
            Assert.Same(HelperContext.Current, Singleton<IHelper>.Instance);
            Assert.NotNull(Singleton<IHelper>.Instance);
        }

        [Fact]
        public void Helper_CreateAnInstance_ByItSelf()
        {
            var helper = HelperContext.Current;

            Assert.Same(helper, Singleton<IHelper>.Instance);
            Assert.Same(helper, BaseSingleton.AllSingletons[typeof(IHelper)]);
            Assert.Same(helper, HelperContext.Current);
            Assert.Same(Singleton<IHelper>.Instance, HelperContext.Current);
        }

        [Fact]
        public void Helper_ShouldReplace_AnInstance()
        {
            var typeFinder = new TypeFinder();
            var helper = HelperContext.Current;
            var newHelper = (IHelper)Activator.CreateInstance(typeFinder.FindClassesOfType<IHelper>().First());

            Assert.Same(helper, HelperContext.Current);
            Assert.NotSame(newHelper, HelperContext.Current);

            HelperContext.Replace(newHelper);

            Assert.Same(newHelper, HelperContext.Current);
        }
    }
}
