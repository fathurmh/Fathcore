namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public virtual long Id { get; set; }
    }
}
