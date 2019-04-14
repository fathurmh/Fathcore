namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for the concurrent entity. The class that implements this, the concurrent will be handled by <see cref="AuditTrail.AuditHandler"/>.
    /// </summary>
    public interface IConcurrentable
    {
        /// <summary>
        /// Gets or sets the entity row version.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
