using System.Reflection;
using Xunit;

namespace Fathcore.DependencyInjection
{
    public class RegisterServiceAttributeTest
    {
        [Fact]
        public void RegisterServiceAttribute_Has_Transient_Lifetime_By_Default()
        {
            var type = typeof(TransientService);
            var result = type.GetCustomAttribute<RegisterServiceAttribute>();

            Assert.Equal(Lifetime.Transient, result.Lifetime);
        }

        [Fact]
        public void Can_Register_Service_With_Specified_Lifetime()
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
