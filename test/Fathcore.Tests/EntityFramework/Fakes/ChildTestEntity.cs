using System;
using System.Collections.Generic;
using Fathcore.EntityFramework;
using Fathcore.EntityFramework.Audit;

namespace Fathcore.Tests.EntityFramework.Fakes
{
    internal class ChildTestEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string ChildTestField { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }
        public TestEntity TestEntity { get; set; }

        public IEnumerable<ChildTestEntity> GenerateData()
        {
            return new List<ChildTestEntity>()
            {
                new ChildTestEntity(){ ChildTestField = "Initial Data 1" },
                new ChildTestEntity(){ ChildTestField = "Initial Data 2" },
                new ChildTestEntity(){ ChildTestField = "Initial Data 3" },
                new ChildTestEntity(){ ChildTestField = "Initial Data 4" },
                new ChildTestEntity(){ ChildTestField = "Initial Data 5" },
            };
        }
    }
}
