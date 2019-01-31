using System.Linq;
using Fathcore.Helpers;
using Fathcore.Infrastructures;
using Fathcore.Providers;
using Fathcore.Providers.Abstractions;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace Fathcore.Tests.Infrastructures
{
    public class TypeFinderTests
    {
        [Fact]
        public void Benchmark_Findings()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            ICoreFileProvider coreFileProvider = new CoreFileProvider(hostingEnvironment.Object);
            var finder = new AppDomainTypeFinder(coreFileProvider);
            var type = finder.FindClassesOfType<ITransientService>().ToList();

            Assert.Equal(4, type.Count);
            Assert.True(typeof(ITransientService).IsAssignableFrom(type.FirstOrDefault()));
        }
    }
}
