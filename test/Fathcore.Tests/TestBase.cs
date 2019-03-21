using System;
using Fathcore.Infrastructure;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            BaseSingleton.AllSingletons.Clear();
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
