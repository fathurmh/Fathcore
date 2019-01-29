using System;
using Xunit;
using Fathcore.Infrastructures;

namespace Fathcore.Tests.Infrastructures
{
    public class SingletonTests
    {
        [Fact]
        public void Singleton_Is_Null_By_Default()
        {
            var instance = Singleton<SingletonTests>.Instance;

            Assert.Null(instance);
        }

        [Fact]
        public void Singletons_Share_Same_Singletons_Dictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Assert.Equal(Singleton<int>.AllSingletons, Singleton<double>.AllSingletons);
            Assert.Equal(1, BaseSingleton.AllSingletons[typeof(int)]);
            Assert.Equal(2.0, BaseSingleton.AllSingletons[typeof(double)]);
        }

        [Fact]
        public void Singleton_Dictionary_Is_Created_By_Default()
        {
            var instance = SingletonDictionary<SingletonTests, object>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void Singleton_Dictionary_Can_Store_Stuff()
        {
            var instance = SingletonDictionary<Type, SingletonTests>.Instance;
            instance[typeof(SingletonTests)] = this;

            Assert.Same(this, instance[typeof(SingletonTests)]);
        }

        [Fact]
        public void Singleton_List_Is_Created_By_Default()
        {
            var instance = SingletonList<SingletonTests>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void Singleton_List_Can_Store_Items()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            instance.Insert(0, this);

            Assert.Same(this, instance[0]);
        }
    }
}
