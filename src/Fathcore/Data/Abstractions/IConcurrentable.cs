namespace Fathcore.Data.Abstractions
{
    /// <summary>
    /// Interface for concurrentable entities
    /// </summary>
    public interface IConcurrentable
    {
        /// <summary>
        /// Gets or sets the entity row version
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
