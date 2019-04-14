using System;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class BaseSingletonTest : IDisposable
    {
        [Fact]
        public void Singletons_Share_Same_Singletons_Dictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Assert.Equal(Singleton<int>.AllSingletons, Singleton<double>.AllSingletons);
            Assert.Equal(1, BaseSingleton.AllSingletons[typeof(int)]);
            Assert.Equal(2.0, BaseSingleton.AllSingletons[typeof(double)]);
        }
        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
