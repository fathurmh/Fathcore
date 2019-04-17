using System;
using System.Collections.Generic;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class SingletonTest : IDisposable
    {
        [Fact]
        public void Singleton_IsNull_ByDefault()
        {
            SingletonTest instance = Singleton<SingletonTest>.Instance;

            Assert.Null(instance);
        }

        [Fact]
        public void Singleton_ShareSame_SingletonsDictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Assert.Equal(Singleton<int>.AllSingletons, Singleton<double>.AllSingletons);
            Assert.Equal(1, BaseSingleton.AllSingletons[typeof(int)]);
            Assert.Equal(2.0, BaseSingleton.AllSingletons[typeof(double)]);
        }

        [Fact]
        public void SingletonList_IsCreated_ByDefault()
        {
            IList<SingletonTest> instance = SingletonList<SingletonTest>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonList_Can_StoreItems()
        {
            IList<SingletonTest> instance = SingletonList<SingletonTest>.Instance;
            instance.Insert(0, this);

            Assert.Same(this, instance[0]);
        }

        [Fact]
        public void SingletonDictionary_IsCreated_ByDefault()
        {
            IDictionary<SingletonTest, object> instance = SingletonDictionary<SingletonTest, object>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonDictionary_Can_StoreStuff()
        {
            IDictionary<Type, SingletonTest> instance = SingletonDictionary<Type, SingletonTest>.Instance;
            instance[typeof(SingletonTest)] = this;

            Assert.Same(this, instance[typeof(SingletonTest)]);
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
