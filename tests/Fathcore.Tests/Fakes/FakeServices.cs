using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Fathcore.Tests.Fakes
{
    public interface ITransientService { }

    [ServiceDescriptor(typeof(ITransientService))]
    public class TransientService1 : ITransientService { }

    public class TransientService2 : ITransientService { }

    public class TransientService : ITransientService { }

    public interface IScopedService { }

    public class ScopedService1 : IScopedService { }

    public class ScopedService2 : IScopedService { }

    public interface IQueryHandler<TQuery, TResult> { }

    public class QueryHandler : IQueryHandler<string, int> { }

    public interface IOpenGeneric<T> : IOtherInheritance { }

    public class OpenGeneric<T> : IOpenGeneric<T> { }

    public interface IPartiallyClosedGeneric<T1, T2> { }

    public class PartiallyClosedGeneric<T> : IPartiallyClosedGeneric<T, int> { }

    public interface ITransientServiceToCombine { }

    public interface IScopedServiceToCombine { }

    public interface ISingletonServiceToCombine { }

    [ServiceDescriptor(typeof(ITransientServiceToCombine))]
    [ServiceDescriptor(typeof(IScopedServiceToCombine), ServiceLifetime.Scoped)]
    [ServiceDescriptor(typeof(ISingletonServiceToCombine), ServiceLifetime.Singleton)]
    public class CombinedService : ITransientServiceToCombine, IScopedServiceToCombine, ISingletonServiceToCombine { }

    public interface IWrongInheritanceA { }

    public interface IWrongInheritanceB { }

    [ServiceDescriptor(typeof(IWrongInheritanceA))]
    public class WrongInheritance : IWrongInheritanceB { }

    public interface IDuplicateInheritance { }

    public interface IOtherInheritance { }

    [ServiceDescriptor(typeof(IOtherInheritance))]
    [ServiceDescriptor(typeof(IDuplicateInheritance))]
    [ServiceDescriptor(typeof(IDuplicateInheritance))]
    public class DuplicateInheritance : IDuplicateInheritance, IOtherInheritance { }
    
    public interface IDefault1 { }

    public interface IDefault2 { }

    public interface IDefault3Level1 { }

    public interface IDefault3Level2 : IDefault3Level1 { }

    [ServiceDescriptor]
    public class DefaultAttributes : IDefault3Level2, IDefault1, IDefault2 { }

    [CompilerGenerated]
    public class CompilerGenerated { }

    public class CombinedService2 : IDefault1, IDefault2, IDefault3Level2 { }

    public interface IUnregisteredService { }
    public class UnregisteredService : IUnregisteredService { public UnregisteredService() { } }
}

namespace Fathcore.Tests.Fakes.ChildNamespace
{
    public class ClassInChildNamespace { }
}

namespace UnwantedNamespace
{
    using Fathcore.Tests.Fakes;
    public class TransientService : ITransientService
    {
    }
}
