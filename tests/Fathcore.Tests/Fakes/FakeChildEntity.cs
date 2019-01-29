using System;
using Fathcore.Data.Abstractions;

namespace Fathcore.Tests.Fakes
{
    public class FakeChildEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string ChildEntityCode { get; set; }
        public string ChildEntityName { get; set; }
        public FakeEntity FakeEntity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
