using System;

namespace Fathcore.Data.Abstractions
{
    /// <summary>
    /// Interface for soft deletable entities
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets or sets the entity deleted is true or false
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the entity deleted time
        /// </summary>
        DateTime? DeletedTime { get; set; }
    }
}
