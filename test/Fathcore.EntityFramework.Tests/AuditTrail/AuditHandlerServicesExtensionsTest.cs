using System;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.EntityFramework.Tests.AuditTrail
{
    public class AuditHandlerServicesExtensionsTest
    {
        IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();

        [Fact]
        public void Should_AddAuditHandler_ToService_Default()
        {
            ServiceDescriptors.AddAuditHandler();
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();

            var result = ServiceDescriptors.Where(p => p.ServiceType == typeof(IAuditHandler));
            var service = serviceProvider.GetRequiredService<IAuditHandler>();

            Assert.NotNull(result);
            Assert.Equal(typeof(AuditHandler), service.GetType());
        }

        [Fact]
        public void Should_AddAuditHandler_ToService_CustomImplementation()
        {
            ServiceDescriptors.AddAuditHandler<CustomAuditHandler>();
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();

            var result = ServiceDescriptors.Where(p => p.ServiceType == typeof(IAuditHandler));
            var service = serviceProvider.GetRequiredService<IAuditHandler>();

            Assert.NotNull(result);
            Assert.NotEqual(typeof(AuditHandler), service.GetType());
            Assert.Equal(typeof(CustomAuditHandler), service.GetType());
        }

        [Fact]
        public void Should_AddAuditHandler_ToService_CustomImplementation_2()
        {
            ServiceDescriptors.AddAuditHandler(typeof(CustomAuditHandler));
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();

            var result = ServiceDescriptors.Where(p => p.ServiceType == typeof(IAuditHandler));
            var service = serviceProvider.GetRequiredService<IAuditHandler>();

            Assert.NotNull(result);
            Assert.NotEqual(typeof(AuditHandler), service.GetType());
            Assert.Equal(typeof(CustomAuditHandler), service.GetType());
        }

        [Fact]
        public void ShouldNot_AddAuditHandler_ToService_WhenImplementationType_IsNotClass()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceDescriptors.AddAuditHandler(typeof(IAuditHandler)));
        }

        [Fact]
        public void ShouldNot_AddAuditHandler_ToService_WhenImplementationType_IsDoesNotAppropriate()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceDescriptors.AddAuditHandler(typeof(AuditHandlerServicesExtensionsTest)));
        }
    }

    class CustomAuditHandler : IAuditHandler
    {
        public void Handle(BaseDbContext dbContext) => throw new NotImplementedException();
        public Task HandleAsync(BaseDbContext dbContext) => throw new NotImplementedException();
    }
}
