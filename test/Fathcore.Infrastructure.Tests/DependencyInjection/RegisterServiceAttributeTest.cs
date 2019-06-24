using System.Reflection;
using Xunit;

namespace Fathcore.Infrastructure.DependencyInjection.Tests
{
    public class RegisterServiceAttributeTest
    {
        [Fact]
        public void RegisterServiceAttribute_HasDefault_TransientLifetime()
        {
            var type = typeof(TransientService);
            var result = type.GetCustomAttribute<RegisterServiceAttribute>();

            Assert.Equal(Lifetime.Transient, result.Lifetime);
        }

        [Fact]
        public void RegisterServiceAttribute_With_SpecifiedLifetime()
        {
            var type = typeof(SingletonService);
            var result = type.GetCustomAttribute<RegisterServiceAttribute>();

            Assert.Equal(Lifetime.Singleton, result.Lifetime);
        }

        [RegisterService]
        private class TransientService { }

        [RegisterService(Lifetime.Singleton)]
        private class SingletonService { }
    }
}
