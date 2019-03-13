using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        private IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        [Fact]
        public void Should_Register_Service_AsSelf()
        {
            ServiceCollection.AddSingleton<Service, Service>().AsSelf();

            var result = ServiceCollection.GetDescriptor<Service>();

            Assert.Equal(typeof(Service), result.ImplementationType);
        }

        [Fact]
        public void Should_Register_Service_AsSelf_Implementation_Type()
        {
            ServiceCollection.AddSingleton<IService, Service>().AsSelf();

            var result = ServiceCollection.GetDescriptor<Service>();
            using (var scope = ServiceCollection.BuildServiceProvider().CreateScope())
            {
                var guid1 = scope.ServiceProvider.GetRequiredService<IService>();
                var guid2 = scope.ServiceProvider.GetRequiredService<Service>();

                Assert.Equal(typeof(Service), result.ImplementationType);
                Assert.Equal(guid1, guid2);
            }
        }

        [Fact]
        public void Should_Register_Service_AsSelf_Implementation_Instance()
        {
            var instance = new Service();
            ServiceCollection.AddSingleton<IService>(instance).AsSelf();

            using (var scope = ServiceCollection.BuildServiceProvider().CreateScope())
            {
                var result = ServiceCollection.GetDescriptor<Service>();
                var guid1 = scope.ServiceProvider.GetRequiredService<IService>();
                var guid2 = scope.ServiceProvider.GetRequiredService<Service>();

                Assert.Equal(instance, result.ImplementationInstance);
                Assert.Equal(guid1, guid2);
            }
        }

        [Fact]
        public void Should_Register_Service_AsSelf_Implementation_Factory()
        {
            ServiceCollection.AddSingleton<IChildService, ChildService>(provider =>
            {
                return new ChildService();
            }).AsSelf();

            using (var scope = ServiceCollection.BuildServiceProvider().CreateScope())
            {
                var guid1 = scope.ServiceProvider.GetRequiredService<IChildService>();
                var guid2 = scope.ServiceProvider.GetRequiredService<ChildService>();

                var result = ServiceCollection.GetDescriptor<IChildService>();

                Assert.NotNull(result.ImplementationFactory);
                Assert.Equal(guid1, guid2);
            }
        }

        private interface IService { }
        private class Service : IService
        {
            public Guid Guid { get; }
            public Service()
            {
                Guid = Guid.NewGuid();
            }
        }
        private interface IChildService { }
        private class ChildService : IChildService
        {
            public Guid Guid { get; }
            public ChildService()
            {
                Guid = Guid.NewGuid();
            }
        }
    }
}
