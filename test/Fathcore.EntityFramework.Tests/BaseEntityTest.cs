using System.Linq;
using Xunit;

namespace Fathcore.EntityFramework.Tests
{
    public class BaseEntityTest
    {
        [Fact]
        public void Id_Property_Should_Have_Long_Integer_Type()
        {
            var entity = new ClassWithLongIntegerGenericType();

            var result = entity.GetType().GetProperties().ToList().ToDictionary(p => p.Name, p => p.PropertyType);

            Assert.Single(result);
            Assert.Equal(typeof(long), result[nameof(entity.Id)]);
        }

        [Fact]
        public void Should_Get_Or_Set()
        {
            var entity = new ClassWithLongIntegerGenericType
            {
                Id = default
            };

            Assert.Equal(default, entity.Id);

            var result = (BaseEntity)entity;
            result.Id = long.MinValue;

            Assert.Equal(long.MinValue, result.Id);
            Assert.Equal(long.MinValue, entity.Id);
        }

        private class ClassWithLongIntegerGenericType : BaseEntity
        {
        }
    }
}
