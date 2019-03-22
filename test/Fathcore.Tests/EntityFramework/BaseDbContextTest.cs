using System.Linq;
using Fathcore.Tests.Fakes;
using Xunit;

namespace Fathcore.Tests.EntityFramework
{
    public class BaseDbContextTest : TestBase
    {
        [Fact]
        public void BaseDbContext_ShouldCreateModels()
        {
            var options = Options("BaseDbContext_ShouldCreateModels");
            using (var context = new TestDbContext(options))
            {
                var result = context.Model.GetEntityTypes();

                Assert.NotEmpty(result);
                Assert.Equal(4, result.Count());
            }
        }
    }
}
