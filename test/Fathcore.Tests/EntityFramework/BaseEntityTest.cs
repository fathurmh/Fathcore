using System.Linq;
using Fathcore.EntityFramework;
using Fathcore.Tests.Fakes;
using Xunit;

namespace Fathcore.Tests.EntityFramework
{
    public class BaseEntityTest
    {
        [Fact]
        public void BaseEntity_Subtitution()
        {
            var entity = FakeEntityGenerator.ClassroomsOnly.First();
            var expectedId = entity.Id = 1;

            BaseEntity<Classroom, int> baseEntity = entity;
            Assert.Equal(expectedId, baseEntity.Id);

            IBaseEntity<int> iBaseEntity = baseEntity;
            Assert.Equal(expectedId, iBaseEntity.Id);

            var entity2 = (Classroom)iBaseEntity;
            Assert.Equal(expectedId, entity2.Id);
        }

        [Fact]
        public void GetEntityType_ShouldPass()
        {
            var expectedType = typeof(Classroom);

            var entity = new Classroom();
            Assert.Equal(expectedType, entity.GetEntityType());

            BaseEntity<Classroom, int> baseEntity = entity;
            Assert.Equal(expectedType, baseEntity.GetEntityType());

            BaseEntity<Classroom> baseEntity2 = baseEntity;
            Assert.Equal(expectedType, baseEntity2.GetEntityType());

            IBaseEntity<int> iBaseEntity = baseEntity;
            Assert.Equal(expectedType, iBaseEntity.GetEntityType());

            IBaseEntity iBaseEntity2 = iBaseEntity;
            Assert.Equal(expectedType, iBaseEntity2.GetEntityType());

            IBaseEntity iBaseEntity3 = baseEntity2;
            Assert.Equal(expectedType, iBaseEntity3.GetEntityType());

            Classroom entity2 = (Classroom)iBaseEntity3;
            Assert.Equal(expectedType, entity2.GetEntityType());
        }

        [Fact]
        public void GetKeyType_ShouldPass()
        {
            var expectedType = typeof(int);

            var entity = new Classroom();
            Assert.Equal(expectedType, entity.GetKeyType());

            BaseEntity<Classroom, int> baseEntity = entity;
            Assert.Equal(expectedType, baseEntity.GetKeyType());

            BaseEntity<Classroom> baseEntity2 = baseEntity;
            Assert.Equal(expectedType, baseEntity2.GetKeyType());

            IBaseEntity<int> iBaseEntity = baseEntity;
            Assert.Equal(expectedType, iBaseEntity.GetKeyType());

            IBaseEntity iBaseEntity2 = iBaseEntity;
            Assert.Equal(expectedType, iBaseEntity2.GetKeyType());

            IBaseEntity iBaseEntity3 = baseEntity2;
            Assert.Equal(expectedType, iBaseEntity3.GetKeyType());

            Classroom entity2 = (Classroom)iBaseEntity3;
            Assert.Equal(expectedType, entity2.GetKeyType());
        }
    }
}
