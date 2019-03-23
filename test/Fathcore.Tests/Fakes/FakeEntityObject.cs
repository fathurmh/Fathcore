using System;
using System.Collections.Generic;
using Fathcore.EntityFramework;

namespace Fathcore.Tests.Fakes
{
    internal class StringQueryType
    {
        public string Value { get; set; }
    }

    [Serializable]
    internal class Classroom : BaseEntity<Classroom, int>, IBaseEntity<int>, IAuditable, ISoftDeletable, IConcurrentable
    {
        public Classroom()
        {
            Students = new HashSet<Student>();
        }

        public string Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }

    [Serializable]
    internal class Student : BaseEntity<Student, int>, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public int ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; }
        public virtual Address Address { get; set; }
    }

    [Serializable]
    internal class Address : BaseEntity<Address, int>, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string Street { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
