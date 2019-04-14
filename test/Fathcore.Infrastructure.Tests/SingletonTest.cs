using System;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class SingletonTest : IDisposable
    {
        [Fact]
        public void Singleton_Is_Null_By_Default()
        {
            SingletonTest instance = Singleton<SingletonTest>.Instance;

            Assert.Null(instance);
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
