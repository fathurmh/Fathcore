using Fathcore.EntityFramework.Configuration;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Configuration
{
    public class QueryTypeConfigurationTest
    {
        [Fact]
        public void Should_Configure_Query_Type()
        {
            var configuration = new StringQueryTypeMapping();
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var mappingConfiguration = (IMappingConfiguration)configuration;
            mappingConfiguration.ApplyConfiguration(modelBuilder);

            IMutableEntityType entityType = modelBuilder.Model.FindEntityType(typeof(StringQueryType));

            Assert.NotNull(entityType);
        }
    }
}
