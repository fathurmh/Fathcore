using System;
using System.Collections.Generic;
using System.Linq;
using Fathcore.EntityFramework.Audit;

namespace Fathcore.EntityFramework.Fakes
{
    internal class TestEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public TestEntity()
        {
            ChildTestEntities = new HashSet<ChildTestEntity>();
        }

        public string TestField { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }
        public virtual ICollection<ChildTestEntity> ChildTestEntities { get; set; }

        public IEnumerable<TestEntity> GenerateData()
        {
            return new List<TestEntity>()
                {
                    new TestEntity(){ TestField = "Initial Data 1", ChildTestEntities = new ChildTestEntity().GenerateData().ToList() },
                    new TestEntity(){ TestField = "Initial Data 2", ChildTestEntities = new ChildTestEntity().GenerateData().ToList() },
                    new TestEntity(){ TestField = "Initial Data 3", ChildTestEntities = new ChildTestEntity().GenerateData().ToList() },
                    new TestEntity(){ TestField = "Initial Data 4", ChildTestEntities = new ChildTestEntity().GenerateData().ToList() },
                    new TestEntity(){ TestField = "Initial Data 5", ChildTestEntities = new ChildTestEntity().GenerateData().ToList() },
                };
        }

        public IEnumerable<TestEntity> GenerateDataWithoutChildren()
        {
            return new List<TestEntity>()
                {
                    new TestEntity(){ TestField = "Initial Data 1" },
                    new TestEntity(){ TestField = "Initial Data 2" },
                    new TestEntity(){ TestField = "Initial Data 3" },
                    new TestEntity(){ TestField = "Initial Data 4" },
                    new TestEntity(){ TestField = "Initial Data 5" },
                };
        }
    }
}
