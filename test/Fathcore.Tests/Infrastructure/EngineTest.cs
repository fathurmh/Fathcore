using System;
using System.Linq;
using Fathcore.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Fathcore.Infrastructure
{
    public class EngineTest
    {
        private IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();
        private IEngine Engine { get; } = new Engine();

        [Fact]
        public void Should_Populate_Service_Collection()
        {
            Engine.Populate(ServiceDescriptors);

            Assert.Contains(ServiceDescriptors, prop => prop.ServiceType.FullName.Contains(nameof(IServiceCollection)));
            Assert.Contains(ServiceDescriptors, prop => prop.ServiceType.FullName.Contains(nameof(ServiceCollection)));

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Service_By_Service_Type()
        {
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.Resolve<IServiceCollection>();

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Service_By_Implementation_Type()
        {
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.Resolve<ServiceCollection>();

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_All_Services_By_Service_Type()
        {
            Engine.Populate(ServiceDescriptors);

            var instances = Engine.ResolveAll<IServiceCollection>();

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_All_Services_By_Implementation_Type()
        {
            Engine.Populate(ServiceDescriptors);

            var instances = Engine.ResolveAll<ServiceCollection>();

            Assert.NotNull(instances);
            Assert.True(instances.Count() > 0);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Unregistered()
        {
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.ResolveUnregistered(typeof(ServiceCollection));

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Not_Resolve_Unregistered_When_Parameter_Service_Not_Registered()
        {
            Engine.Populate(ServiceDescriptors);

            Assert.Throws<Exception>(() => Engine.ResolveUnregistered(typeof(UnregisteredService)));

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Service_By_Service_Type_When_HttpContextAccessor_Exists()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            ServiceDescriptors.AddSingleton(ServiceDescriptors);
            context.RequestServices = ServiceDescriptors.BuildServiceProvider();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            ServiceDescriptors.AddSingleton(mockHttpContextAccessor.Object);
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.Resolve<IServiceCollection>();

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Service_By_Service_Type_When_HttpContextAccessor_Exists_But_HttpContext_Doesnt()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context = null;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            ServiceDescriptors.AddSingleton(mockHttpContextAccessor.Object);
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.Resolve<IServiceCollection>();

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Fact]
        public void Can_Resolve_Service_By_Service_Type_When_HttpContextAccessor_Exists_But_RequestServices_Not()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.RequestServices = null;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            ServiceDescriptors.AddSingleton(mockHttpContextAccessor.Object);
            Engine.Populate(ServiceDescriptors);

            var instance = Engine.Resolve<IServiceCollection>();

            Assert.NotNull(instance);

            Fathcore.Engine.Replace(new Engine());
        }

        [Theory]
        [InlineData(typeof(SelfServiceTest), typeof(SelfServiceTest))]
        [InlineData(typeof(IImplementedServiceTest), typeof(ImplementedServiceTest))]
        public void Should_Activate_Dependency_Attribute_Registrar(Type serviceType, Type implementedType)
        {
            var typeFinder = new TypeFinder();
            typeFinder.AssemblyNames.Add("Fathcore.Tests");

            var engine = new Engine().With(typeFinder);
            engine.Populate(ServiceDescriptors);

            var instance = engine.Resolve(serviceType);

            Assert.Equal(implementedType, instance.GetType());

            Fathcore.Engine.Replace(new Engine());
        }

        private class DependencyRegistrarSingletonTest : IDependencyRegistrar
        {
            public IServiceCollection Register(IServiceCollection services)
                => services.AddSingleton<IDependencyRegistrar, DependencyRegistrarSingletonTest>();
        }

        [RegisterService(Lifetime.Singleton)]
        private class SelfServiceTest { }

        private interface IImplementedServiceTest { }

        [RegisterService(Lifetime.Singleton)]
        private class ImplementedServiceTest : IImplementedServiceTest { }

        private interface IUnregisteredService { }

        private class UnregisteredService
        {
            private readonly IUnregisteredService _unregisteredService;

            public UnregisteredService(IUnregisteredService unregisteredService)
            {
                _unregisteredService = unregisteredService;
            }
        }
    }
}
