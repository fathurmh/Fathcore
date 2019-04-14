using System;
using System.Collections.Generic;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class SingletonListTest : IDisposable
    {
        [Fact]
        public void Singleton_List_Is_Created_By_Default()
        {
            IList<SingletonListTest> instance = SingletonList<SingletonListTest>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void Singleton_List_Can_Store_Items()
        {
            IList<SingletonListTest> instance = SingletonList<SingletonListTest>.Instance;
            instance.Insert(0, this);

            Assert.Same(this, instance[0]);
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
