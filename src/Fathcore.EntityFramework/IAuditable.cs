using System;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for the auditable entity. The class that implements this can be audited by <see cref="AuditTrail.AuditHandler"/>.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets or sets the entity created by.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the entity created time.
        /// </summary>
        DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the entity modified by.
        /// </summary>
        string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the entity modified time.
        /// </summary>
        DateTime? ModifiedTime { get; set; }
    }
}
