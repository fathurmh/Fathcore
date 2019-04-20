using System;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.EntityFramework.Builders;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework.Tests.Fakes
{
    internal class TestDbContext : BaseDbContext, IDbContext
    {
        public TestDbContext(DbContextOptions options, IAuditHandler auditHandler = default)
            : base(options, auditHandler)
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
