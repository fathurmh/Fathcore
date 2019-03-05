namespace Fathcore.EntityFramework.Audit
{
    /// <summary>
    /// Concurrentable entity interface.
    /// </summary>
    public partial interface IConcurrentable
    {
        /// <summary>
        /// Gets or sets the entity row version.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
