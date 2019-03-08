using System.Linq;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class TypeFinderTest
    {
        [Fact]
        public void Should_Found_Classes()
        {
            var finder = new TypeFinder();
            var type = finder.FindClassesOfType<ITypeFinder>().ToList();

            Assert.Single(type);
            Assert.True(typeof(ITypeFinder).IsAssignableFrom(type.Single()));
        }
    }
}
