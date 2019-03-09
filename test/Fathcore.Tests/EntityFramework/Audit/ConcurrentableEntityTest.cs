using System.Linq;
using Fathcore.EntityFramework.Audit;
using Xunit;

namespace Fathcore.Tests.EntityFramework.Audit
{
    public class ConcurrentableEntityTest
    {
        [Fact]
        public void Entity_Should_Have_Concurrentable_Property()
        {
            var entity = new ConcurrentableClass();

            var result = entity.GetType().GetProperties().ToList().ToDictionary(p => p.Name, p => p.PropertyType);

            Assert.Single(result);
            Assert.Equal(typeof(byte[]), result[nameof(entity.RowVersion)]);
        }

        [Fact]
        public void Should_Get_Or_Set()
        {
            var entity = new ConcurrentableClass
            {
                RowVersion = default
            };

            Assert.Equal(default, entity.RowVersion);

            var result = (IConcurrentable)entity;
            result.RowVersion = new[] { byte.MinValue, byte.MaxValue };

            Assert.Equal(new[] { byte.MinValue, byte.MaxValue }, result.RowVersion);
            Assert.Equal(new[] { byte.MinValue, byte.MaxValue }, entity.RowVersion);
        }

        private class ConcurrentableClass : IConcurrentable
        {
            public byte[] RowVersion { get; set; }
        }
    }
}
