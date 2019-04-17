using System;
using Fathcore.EntityFramework.Builders;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework.Tests.Fakes
{
    internal class TestDbContext : BaseDbContext, IDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mappingTypes = new Type[]
            {
                typeof(ClassroomMapping),
                typeof(StudentMapping),
                typeof(AddressMapping),
                typeof(StringQueryTypeMapping)
            };

            foreach (var type in mappingTypes)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(type);
                configuration.ApplyConfiguration(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
