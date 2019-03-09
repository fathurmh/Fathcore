using Fathcore.Infrastructure;
using Xunit;

namespace Fathcore.Tests.Infrastructure
{
    public class SingletonTest
    {
        [Fact]
        public void Singleton_Is_Null_By_Default()
        {
            SingletonTest instance = Singleton<SingletonTest>.Instance;

            Assert.Null(instance);
        }

        [Fact]
        public void Singleton_Share_Same_Singletons_Dictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Assert.Equal(Singleton<int>.AllSingletons, Singleton<double>.AllSingletons);
            Assert.Equal(1, BaseSingleton.AllSingletons[typeof(int)]);
            Assert.Equal(2.0, BaseSingleton.AllSingletons[typeof(double)]);
        }
    }
}
