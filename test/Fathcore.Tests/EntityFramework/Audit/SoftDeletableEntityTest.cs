using System;
using System.Linq;
using Xunit;

namespace Fathcore.EntityFramework.AuditTrail
{
    public class SoftDeletableEntityTest
    {
        [Fact]
        public void Entity_Should_Have_SoftDeletable_Property()
        {
            var entity = new SoftDeletableClass();

            var result = entity.GetType().GetProperties().ToList().ToDictionary(p => p.Name, p => p.PropertyType);

            Assert.Equal(2, result.Count);
            Assert.Equal(typeof(bool), result[nameof(entity.IsDeleted)]);
            Assert.Equal(typeof(DateTime?), result[nameof(entity.DeletedTime)]);
        }

        [Fact]
        public void Should_Get_Or_Set()
        {
            var entity = new SoftDeletableClass
            {
                IsDeleted = default,
                DeletedTime = default
            };

            Assert.Equal(default, entity.IsDeleted);
            Assert.Equal(default, entity.DeletedTime);

            var result = (ISoftDeletable)entity;
            result.IsDeleted = true;
            result.DeletedTime = DateTime.MinValue;

            Assert.True(result.IsDeleted);
            Assert.True(entity.IsDeleted);
            Assert.Equal(DateTime.MinValue, result.DeletedTime);
            Assert.Equal(DateTime.MinValue, entity.DeletedTime);
        }

        private class SoftDeletableClass : ISoftDeletable
        {
            public bool IsDeleted { get; set; }
            public DateTime? DeletedTime { get; set; }
        }
    }
}
