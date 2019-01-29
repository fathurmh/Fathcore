using System;
using System.Collections.Generic;
using Fathcore.Data.Abstractions;
using Fathcore.Tests.Fakes;

namespace Fathcore.Tests.Fakes
{
    public class FakeEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public FakeEntity()
        {
            FakeChildEntities = new HashSet<FakeChildEntity>();
        }
        
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }
        public virtual ICollection<FakeChildEntity> FakeChildEntities { get; set; }
    }
}
