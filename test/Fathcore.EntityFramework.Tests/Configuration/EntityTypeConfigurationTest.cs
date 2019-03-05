using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fathcore.EntityFramework.Configuration;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Configuration
{
    public class EntityTypeConfigurationTest
    {
        [Fact]
        public void Derived_Entity_From_BaseEntity_Should_Configured()
        {
            var configuration = new TestEntityMapping();
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var mappingConfiguration = (IMappingConfiguration)configuration;
            mappingConfiguration.ApplyConfiguration(modelBuilder);

            IMutableEntityType entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            IMutableKey key = entityType.GetKeys().Single();
            IMutableProperty keyInfo = key.Properties.Single();

            Assert.Equal(nameof(TestEntity.Id), keyInfo.Name);
            Assert.True(keyInfo.IsPrimaryKey());
            Assert.True(keyInfo.IsKey());
        }

        [Fact]
        public void Derived_Entity_From_BaseEntity_And_Implements_IAuditable_Should_Configured()
        {
            var configuration = new TestEntityMapping();
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var mappingConfiguration = (IMappingConfiguration)configuration;
            mappingConfiguration.ApplyConfiguration(modelBuilder);

            IMutableEntityType entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            IEnumerable<IMutableProperty> entityProperties = entityType.GetProperties();
            PropertyInfo[] clrProperties = typeof(TestEntity).GetProperties();

            Assert.Equal(9, entityProperties.Count());
            Assert.Contains(entityProperties, p => clrProperties.Any(q => p.Name == q.Name));
        }

        [Fact]
        public void Derived_Entity_From_BaseEntity_And_Implements_ISoftDeletable_Should_Configured()
        {
            var configuration = new TestEntityMapping();
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var mappingConfiguration = (IMappingConfiguration)configuration;
            mappingConfiguration.ApplyConfiguration(modelBuilder);

            IMutableEntityType entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            IEnumerable<IMutableProperty> entityProperties = entityType.GetProperties();
            PropertyInfo[] clrProperties = typeof(TestEntity).GetProperties();

            Assert.Equal(9, entityProperties.Count());
            Assert.Contains(entityProperties, p => clrProperties.Any(q => p.Name == q.Name));
        }

        [Fact]
        public void Derived_Entity_From_BaseEntity_And_Implements_IConcurrentable_Should_Configured()
        {
            var configuration = new TestEntityMapping();
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var mappingConfiguration = (IMappingConfiguration)configuration;
            mappingConfiguration.ApplyConfiguration(modelBuilder);

            IMutableEntityType entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            IEnumerable<IMutableProperty> entityProperties = entityType.GetProperties();
            PropertyInfo[] clrProperties = typeof(TestEntity).GetProperties();

            Assert.Equal(9, entityProperties.Count());
            Assert.Contains(entityProperties, p => clrProperties.Any(q => p.Name == q.Name));
        }
    }
}
