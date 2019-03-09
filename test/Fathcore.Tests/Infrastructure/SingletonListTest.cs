using System.Collections.Generic;
using Fathcore.Infrastructure;
using Xunit;

namespace Fathcore.Tests.Infrastructure
{
    public class SingletonListTest
    {
        [Fact]
        public void SingletonList_Is_Created_By_Default()
        {
            IList<SingletonListTest> instance = SingletonList<SingletonListTest>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonList_Can_Store_Items()
        {
            IList<SingletonListTest> instance = SingletonList<SingletonListTest>.Instance;
            instance.Insert(0, this);

            Assert.Same(this, instance[0]);
        }
    }
}
