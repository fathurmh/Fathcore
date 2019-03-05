using System;

namespace Fathcore.EntityFramework.Audit
{
    /// <summary>
    /// Auditable entity interface.
    /// </summary>
    public partial interface IAuditable
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
