using System;
using System.Collections.Generic;
using Fathcore.EntityFramework.Audit;

namespace Fathcore.EntityFramework.Fakes
{
    internal class StringQueryType
    {
        public string Value { get; set; }
    }

    internal class AuthorEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public AuthorEntity()
        {
            Books = new HashSet<BookEntity>();
        }

        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<BookEntity> Books { get; set; }
    }

    internal class BookEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AuthorEntity Writer { get; set; }
        public virtual TitleEntity Title { get; set; }
    }

    internal class TitleEntity : BaseEntity, IAuditable, ISoftDeletable, IConcurrentable
    {
        public string Subject { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual BookEntity Book { get; set; }
    }
}
