using System;
using Fathcore.Infrastructure;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            Singleton<IEngine>.Instance = null;
        }

        public void Dispose()
        {
            Engine.Replace(new Infrastructure.Engine());
        }
    }
}
