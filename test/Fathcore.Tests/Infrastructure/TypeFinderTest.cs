using System;
using System.Linq;
using Xunit;

namespace Fathcore.Infrastructure
{
    public class TypeFinderTest
    {
        public TypeFinder TypeFinder { get; } = new TypeFinder();

        [Theory]
        [InlineData("Fathcore", true)]
        [InlineData("Fathcore.EntityFramework", true)]
        [InlineData("System", false)]
        public void Should_Check_Assembly_Matches(string assemblyName, bool expected)
        {
            var result = TypeFinder.Matches(assemblyName);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Should_Found_Class_By_Interface()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");

            var type = TypeFinder.FindClassesOfType<IInterface>(false).ToList();

            Assert.Single(type);
            Assert.True(typeof(IInterface).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Should_Found_Class_By_Implemented()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");

            var type = TypeFinder.FindClassesOfType(typeof(Implemented)).ToList();

            Assert.Single(type);
            Assert.True(typeof(IInterface).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Should_Found_Class_By_Interface_With_Specified_Assemblies()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");

            var type = TypeFinder.FindClassesOfType<IInterface>(TypeFinder.GetAssemblies(), false).ToList();

            Assert.Single(type);
            Assert.True(typeof(IInterface).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Should_Found_Class_By_Interface_Closed_Generic()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");

            var type = TypeFinder.FindClassesOfType(typeof(IGenericInterface<int>)).ToList();

            Assert.Single(type);
            Assert.True(typeof(IGenericInterface<int>).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Should_Found_Class_By_Interface_Open_Generic()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");

            var type = TypeFinder.FindClassesOfType(typeof(IGenericInterface<>)).ToList();

            Assert.Single(type);
            Assert.True(typeof(GenericImplemented).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Should_Found_Classes_With_Attribute()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");
            var assemblies = TypeFinder.GetAssemblies();

            var type = TypeFinder.FindClassesWithAttribute<CustomAttribute>(assemblies, false).ToList();

            Assert.Single(type);
            Assert.True(typeof(IInterface).IsAssignableFrom(type.Single()));
        }

        [Fact]
        public void Scanned_Assembly_Should_Not_Duplicated()
        {
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");
            TypeFinder.AssemblyNames.Add("Fathcore.Tests");
            var assemblies = TypeFinder.GetAssemblies();

            Assert.True(assemblies.Distinct().Count() == assemblies.Count());
        }

        internal interface IInterface { }
        [Custom]
        internal class Implemented : IInterface { }
        internal interface IGenericInterface<T> { }
        internal class GenericImplemented : IGenericInterface<int> { }
        internal class CustomAttribute : Attribute { }
    }
}
