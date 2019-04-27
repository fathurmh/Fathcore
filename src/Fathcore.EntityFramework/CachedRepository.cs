using System.Collections.Generic;
using Fathcore.Infrastructure.Caching;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the class for generic cached repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public class CachedRepository<TEntity> : ICachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ICacheSetting _cacheSetting;

        public CachedRepository(IRepository<TEntity> repository, IStaticCacheManager cacheManager, ICacheSetting cacheSetting)
        {
            _repository = repository.AsNoTracking();
            _cacheManager = cacheManager;
            _cacheSetting = cacheSetting;
        }

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        public IEnumerable<TEntity> SelectList()
        {
            var key = string.Join(".", _cacheSetting.CachePrefix, typeof(TEntity).Name, nameof(SelectList));
            return _cacheManager.Get(key, () => _repository.SelectList(), _cacheSetting.CacheTime);
        }
    }
}
