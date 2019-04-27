using System.Collections.Generic;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for generic cached repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public interface ICachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList();
    }
}
