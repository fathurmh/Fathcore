using System;
using System.Collections.Generic;
using Fathcore.EntityFramework;
using Fathcore.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.Tests.EntityFramework.Fakes
{
    internal class TestDbContext : BaseDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = new List<Type>()
            {
                typeof(TestEntityMapping),
                typeof(ChildTestEntityMapping),
                typeof(StringQueryTypeMapping)
            };

            foreach (var type in types)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(type);
                configuration.ApplyConfiguration(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
