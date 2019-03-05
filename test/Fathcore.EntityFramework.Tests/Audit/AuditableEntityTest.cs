using System;
using System.Linq;
using Fathcore.EntityFramework.Audit;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Audit
{
    public class AuditableEntityTest
    {
        [Fact]
        public void Entity_Should_Have_Auditable_Property()
        {
            var entity = new AuditableClass();

            var result = entity.GetType().GetProperties().ToList().ToDictionary(p => p.Name, p => p.PropertyType);

            Assert.Equal(4, result.Count);
            Assert.Equal(typeof(string), result[nameof(entity.CreatedBy)]);
            Assert.Equal(typeof(DateTime), result[nameof(entity.CreatedTime)]);
            Assert.Equal(typeof(string), result[nameof(entity.ModifiedBy)]);
            Assert.Equal(typeof(DateTime?), result[nameof(entity.ModifiedTime)]);
        }

        [Fact]
        public void Should_Get_Or_Set()
        {
            var entity = new AuditableClass
            {
                CreatedBy = default,
                CreatedTime = default,
                ModifiedBy = default,
                ModifiedTime = default,
            };

            Assert.Equal(default, entity.CreatedBy);
            Assert.Equal(default, entity.CreatedTime);
            Assert.Equal(default, entity.ModifiedBy);
            Assert.Equal(default, entity.ModifiedTime);

            var result = (IAuditable)entity;
            result.CreatedBy = "Test";
            result.CreatedTime = DateTime.Today;
            result.ModifiedBy = null;
            result.ModifiedTime = null;

            Assert.Equal("Test", result.CreatedBy);
            Assert.Equal("Test", entity.CreatedBy);
            Assert.Equal(DateTime.Today, result.CreatedTime);
            Assert.Equal(DateTime.Today, entity.CreatedTime);
            Assert.Null(result.ModifiedBy);
            Assert.Null(entity.ModifiedBy);
            Assert.Null(result.ModifiedTime);
            Assert.Null(entity.ModifiedTime);
        }

        private class AuditableClass : IAuditable
        {
            public string CreatedBy { get; set; }
            public DateTime CreatedTime { get; set; }
            public string ModifiedBy { get; set; }
            public DateTime? ModifiedTime { get; set; }
        }
    }
}
