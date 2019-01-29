using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fathcore.Infrastructures;
using Fathcore.Infrastructures.Abstractions;
using Fathcore.Tests;
using Fathcore.Tests.Fakes;
using Fathcore.Tests.Fakes.ChildNamespace;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Xunit;

namespace Fathcore.Tests.Infrastructures
{
    public class DependencyRegistrarTests : TestsBase
    {
        private IServiceCollection Collection { get; } = new ServiceCollection();

        [Fact]
        public void Should_Register_Services()
        {
            Collection.AddSingleton<IDependencyRegistrar, DependencyRegistrar>();
            var serviceProvider = Collection.BuildServiceProvider();
            var requestedService = serviceProvider.GetRequiredService<IDependencyRegistrar>();
            var registeredServices = EngineContext.Current.ServiceCollection.GetDescriptors(ServiceLifetime.Singleton).ToList();

            Assert.True(registeredServices.Count > 0);
            Assert.Contains(registeredServices, prop => prop.ServiceType.FullName.Contains(nameof(IDependencyRegistrar)));
        }

        [Fact]
        public void Scan_These_Types()
        {
            Collection.Scan(scan => scan
                .AddTypes<TransientService1, TransientService2>()
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime());

            Assert.Equal(2, Collection.Count);

            Assert.All(Collection, x =>
            {
                Assert.Equal(ServiceLifetime.Singleton, x.Lifetime);
                Assert.Equal(typeof(ITransientService), x.ServiceType);
            });
        }

        [Fact]
        public void Using_Registration_Strategy_None()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(8, services.Count(x => x.ServiceType == typeof(ITransientService)));
        }

        [Fact]
        public void Using_Registration_Strategy_Skip_If_Exists()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(4, services.Count(x => x.ServiceType == typeof(ITransientService)));
        }

        [Fact]
        public void Using_Registration_Strategy_Replace_Default()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .UsingRegistrationStrategy(RegistrationStrategy.Replace())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(1, services.Count(x => x.ServiceType == typeof(ITransientService)));
        }

        [Fact]
        public void Using_Registration_Strategy_Replace_Service_Types()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType))
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(1, services.Count(x => x.ServiceType == typeof(ITransientService)));
        }

        [Fact]
        public void Using_Registration_Strategy_Replace_Implementation_Types()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(4, services.Count(x => x.ServiceType == typeof(ITransientService)));
        }

        [Fact]
        public void Can_Filter_Types_To_Scan()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<ITransientService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(services, Collection);

            Assert.All(services, service =>
            {
                Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
                Assert.Equal(typeof(ITransientService), service.ServiceType);
            });
        }

        [Fact]
        public void Can_Register_As_Specific_Type()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses(classes => classes.AssignableTo<ITransientService>())
                    .As<ITransientService>());

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.Equal(services, Collection);

            Assert.All(services, service =>
            {
                Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
                Assert.Equal(typeof(ITransientService), service.ServiceType);
            });
        }

        [Fact]
        public void Can_Specify_Lifetime()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<IScopedService>()
                .AddClasses(classes => classes.AssignableTo<IScopedService>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            var services = Collection.GetDescriptors<IScopedService>();

            Assert.Equal(services, Collection);

            Assert.All(services, service =>
            {
                Assert.Equal(ServiceLifetime.Scoped, service.Lifetime);
                Assert.Equal(typeof(IScopedService), service.ServiceType);
            });
        }

        [Fact]
        public void Can_Register_Generic_Types()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<IScopedService>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            var service = Collection.GetDescriptor<IQueryHandler<string, int>>();

            Assert.NotNull(service);
            Assert.Equal(ServiceLifetime.Scoped, service.Lifetime);
            Assert.Equal(typeof(QueryHandler), service.ImplementationType);
        }

        [Fact]
        public void Can_Scan_Using_Attributes()
        {
            var interfaces = new[]
            {
                typeof(ITransientService),
                typeof(ITransientServiceToCombine),
                typeof(IScopedServiceToCombine),
                typeof(ISingletonServiceToCombine),

            };

            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses(t => t.AssignableToAny(interfaces))
                    .UsingAttributes());

            Assert.Equal(4, Collection.Count);

            var service = Collection.GetDescriptor<ITransientService>();

            Assert.NotNull(service);
            Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
            Assert.Equal(typeof(TransientService1), service.ImplementationType);
        }

        [Fact]
        public void Can_Filter_Attribute_Types()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses(t => t.AssignableTo<ITransientService>())
                    .UsingAttributes());

            Assert.Equal(1, Collection.Count);

            var service = Collection.GetDescriptor<ITransientService>();

            Assert.NotNull(service);
            Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
            Assert.Equal(typeof(TransientService1), service.ImplementationType);
        }

        [Fact]
        public void Can_Create_Default()
        {
            var types = new[]
            {
                typeof(IDefault1),
                typeof(IDefault2),
                typeof(IDefault3Level1),
                typeof(IDefault3Level2)
            };

            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses(t => t.AssignableTo<DefaultAttributes>())
                    .UsingAttributes());

            var remainingSetOfTypes = Collection
                .Select(descriptor => descriptor.ServiceType)
                .Except(types.Concat(new[] { typeof(DefaultAttributes) }))
                .ToList();

            Assert.Equal(5, Collection.Count);
            Assert.Empty(remainingSetOfTypes);
        }

        [Fact]
        public void Throws_On_Wrong_Inheritance()
        {
            var collection = new ServiceCollection();

            var ex = Assert.Throws<InvalidOperationException>(() =>
                collection.Scan(scan => scan.FromAssemblyOf<IWrongInheritanceA>()
                    .AddClasses()
                        .UsingAttributes()));

            Assert.Equal(@"Type ""Fathcore.Tests.Fakes.WrongInheritance"" is not assignable to ""Fathcore.Tests.Fakes.IWrongInheritanceA"".", ex.Message);
        }

        [Fact]
        public void Throws_On_Duplicate()
        {
            var collection = new ServiceCollection();

            var ex = Assert.Throws<InvalidOperationException>(() =>
                collection.Scan(scan => scan.FromAssemblyOf<IDuplicateInheritance>()
                    .AddClasses(t => t.AssignableTo<IDuplicateInheritance>())
                        .UsingAttributes()));

            Assert.Equal(@"Type ""Fathcore.Tests.Fakes.DuplicateInheritance"" has multiple ServiceDescriptor attributes with the same service type.", ex.Message);
        }

        [Fact]
        public void Can_Handle_Multiple_Attributes()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<ITransientServiceToCombine>()
                .AddClasses(t => t.AssignableTo<ITransientServiceToCombine>())
                    .UsingAttributes());

            var transientService = Collection.GetDescriptor<ITransientServiceToCombine>();

            Assert.NotNull(transientService);
            Assert.Equal(ServiceLifetime.Transient, transientService.Lifetime);
            Assert.Equal(typeof(CombinedService), transientService.ImplementationType);

            var scopedService = Collection.GetDescriptor<IScopedServiceToCombine>();

            Assert.NotNull(scopedService);
            Assert.Equal(ServiceLifetime.Scoped, scopedService.Lifetime);
            Assert.Equal(typeof(CombinedService), scopedService.ImplementationType);

            var singletonService = Collection.GetDescriptor<ISingletonServiceToCombine>();

            Assert.NotNull(singletonService);
            Assert.Equal(ServiceLifetime.Singleton, singletonService.Lifetime);
            Assert.Equal(typeof(CombinedService), singletonService.ImplementationType);
        }

        [Fact]
        public void Auto_Register_As_Matching_Interface()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses()
                    .AsMatchingInterface()
                    .WithTransientLifetime());

            Assert.Equal(5, Collection.Count);

            var services = Collection.GetDescriptors<ITransientService>();

            Assert.NotNull(services);
            Assert.All(services, s =>
            {
                Assert.Equal(ServiceLifetime.Transient, s.Lifetime);
                Assert.Equal(typeof(ITransientService), s.ServiceType);
            });
        }

        [Fact]
        public void Auto_Register_As_Matching_Interface_Same_Namespace_Only()
        {
            Collection.Scan(scan => scan.FromAssemblyOf<ITransientService>()
                .AddClasses()
                    .AsMatchingInterface((t, x) => x.InNamespaces(t.Namespace))
                    .WithTransientLifetime());

            Assert.Equal(4, Collection.Count);

            var service = Collection.GetDescriptor<ITransientService>();

            Assert.NotNull(service);
            Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
            Assert.Equal(typeof(TransientService), service.ImplementationType);
        }

        [Fact]
        public void Should_Register_Open_Generic_Types()
        {
            var genericTypes = new[]
            {
                typeof(OpenGeneric<>),
                typeof(PartiallyClosedGeneric<>)
            };

            Collection.Scan(scan => scan
                .AddTypes(genericTypes)
                    .AddClasses()
                    .AsImplementedInterfaces());

            var provider = Collection.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IOpenGeneric<int>>());
            Assert.NotNull(provider.GetService<IOpenGeneric<string>>());

            // We don't register partially closed generic types.
            Assert.Null(provider.GetService<IPartiallyClosedGeneric<string, int>>());
        }

        [Fact]
        public void Should_Not_Include_Compiler_Generated_Types()
        {
            Assert.Empty(Collection.Scan(scan => scan.AddType<CompilerGenerated>()));
        }

        [Fact]
        public void Should_Not_Register_Types_In_Sub_Namespace()
        {
            Collection.Scan(scan => scan.FromAssembliesOf(GetType())
                .AddClasses(classes => classes.InExactNamespaceOf<ITransientService>())
                .AsSelf());

            var provider = Collection.BuildServiceProvider();

            Assert.Null(provider.GetService<ClassInChildNamespace>());
        }

        [Fact]
        public void Scan_Should_Create_Separate_Registrations_Per_Interface()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<CombinedService2>()
                .AddClasses(classes => classes.AssignableTo<CombinedService2>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<CombinedService2>())
                    .AsSelf()
                    .WithSingletonLifetime());

            Assert.Equal(5, Collection.Count);

            Assert.All(Collection, x =>
            {
                Assert.Equal(ServiceLifetime.Singleton, x.Lifetime);
                Assert.Equal(typeof(CombinedService2), x.ImplementationType);
            });
        }

        [Fact]
        public void As_Self_With_Interfaces_Should_Forward_Registrations_To_Class()
        {
            Collection.Scan(scan => scan
                .FromAssemblyOf<CombinedService2>()
                .AddClasses(classes => classes.AssignableTo<CombinedService2>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime());

            Assert.Equal(5, Collection.Count);

            var service1 = Collection.GetDescriptor<CombinedService2>();

            Assert.NotNull(service1);
            Assert.Equal(ServiceLifetime.Singleton, service1.Lifetime);
            Assert.Equal(typeof(CombinedService2), service1.ImplementationType);

            var interfaceDescriptors = Collection.Where(x => x.ImplementationType != typeof(CombinedService2)).ToList();
            Assert.Equal(4, interfaceDescriptors.Count);

            Assert.All(interfaceDescriptors, x =>
            {
                Assert.Equal(ServiceLifetime.Singleton, x.Lifetime);
                Assert.NotNull(x.ImplementationFactory);
            });
        }

        [Fact]
        public void As_Self_With_Interfaces_Should_Create_True_Singletons()
        {
            var provider = ConfigureProvider(services =>
            {
                services.Scan(scan => scan
                    .FromAssemblyOf<CombinedService2>()
                     .AddClasses(classes => classes.AssignableTo<CombinedService2>())
                        .AsSelfWithInterfaces()
                        .WithSingletonLifetime());
            });

            var instance1 = provider.GetRequiredService<CombinedService2>();
            var instance2 = provider.GetRequiredService<IDefault1>();
            var instance3 = provider.GetRequiredService<IDefault2>();
            var instance4 = provider.GetRequiredService<IDefault3Level2>();
            var instance5 = provider.GetRequiredService<IDefault3Level1>();

            Assert.Same(instance1, instance2);
            Assert.Same(instance1, instance3);
            Assert.Same(instance1, instance4);
            Assert.Same(instance1, instance5);
        }
    }
}
