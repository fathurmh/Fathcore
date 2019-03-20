namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Concurrentable entity interface.
    /// </summary>
    public interface IConcurrentable
    {
        /// <summary>
        /// Gets or sets the entity row version.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
