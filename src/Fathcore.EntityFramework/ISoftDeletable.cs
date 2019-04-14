using System;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for the soft-deletable entity. The class that implements this, the delete action will be handled by <see cref="AuditTrail.AuditHandler"/>.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets or sets the entity deleted is true or false.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the entity deleted time.
        /// </summary>
        DateTime? DeletedTime { get; set; }
    }
}
