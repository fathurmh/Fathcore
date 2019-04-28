using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Infrastructure.Caching;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the class for generic cached repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public class CachedRepository<TEntity> : ICachedRepository<TEntity>, IAsyncCachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ICacheSetting _cacheSetting;
        private readonly string _cachePattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="repository">An instance of <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="cacheManager">An instance of <see cref="IStaticCacheManager"/>.</param>
        /// <param name="cacheSetting">An instance of <see cref="ICacheSetting"/>.</param>
        public CachedRepository(IRepository<TEntity> repository, IStaticCacheManager cacheManager, ICacheSetting cacheSetting)
        {
            _repository = repository.AsNoTracking();
            _cacheManager = cacheManager;
            _cacheSetting = cacheSetting;
            _cachePattern = string.Join(".", _cacheSetting.CachePrefix, typeof(TEntity).Name);
        }

        /// <summary>
        /// Select all entities. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList()
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList));
            return _cacheManager.Get(key, () => _repository.SelectList(), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.Select(p => p.Name)));
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties));
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name);
            return _cacheManager.Get(key, () => _repository.SelectList(predicate), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, string.Join(".", navigationProperties.Select(p => p.Name)));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, string.Join(".", navigationProperties));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name);
            return _cacheManager.Get(key, () => _repository.Select(predicate), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, string.Join(".", navigationProperties.Select(p => p.Name)));
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, string.Join(".", navigationProperties));
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual TEntity Select(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            var key = string.Join(".", _cachePattern, nameof(Select), keyValue.ToString());
            return _cacheManager.Get(key, () => _repository.Select(keyValue), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual TEntity Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _repository.Insert(entity);
        }

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            return _repository.Insert(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already exists in cache.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual TEntity Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _repository.Update(entity);
        }

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already exists in cache.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            return _repository.Update(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        public virtual void Delete(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            _repository.Delete(keyValue);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _repository.Delete(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            _repository.Delete(entities);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public virtual int SaveChanges()
        {
            _cacheManager.RemoveByPattern(_cachePattern);

            return _repository.SaveChanges();
        }

        /// <summary>
        /// Select all entities. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync()
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync), string.Join(".", navigationProperties.Select(p => p.Name)));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync), string.Join(".", navigationProperties));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync), predicate.Name);
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(predicate), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync), predicate.Name, string.Join(".", navigationProperties.Select(p => p.Name)));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectListAsync), predicate.Name, string.Join(".", navigationProperties));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectAsync), predicate.Name);
            return await _cacheManager.GetAsync(key, () => _repository.SelectAsync(predicate), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectAsync), predicate.Name, string.Join(".", navigationProperties.Select(p => p.Name)));
            return await _cacheManager.GetAsync(key, () => _repository.SelectAsync(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectAsync), predicate.Name, string.Join(".", navigationProperties));
            return await _cacheManager.GetAsync(key, () => _repository.SelectAsync(predicate, navigationProperties), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(object keyValue)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectAsync), keyValue.ToString());
            return await _cacheManager.GetAsync(key, () => _repository.SelectAsync(keyValue), _cacheSetting.CacheTime);
        }

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _repository.InsertAsync(entity);
        }

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            return await _repository.InsertAsync(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already exists in cache.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already exists in cache.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            return await _repository.UpdateAsync(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            await _repository.DeleteAsync(keyValue);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _repository.DeleteAsync(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await _repository.DeleteAsync(entities);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            _cacheManager.RemoveByPattern(_cachePattern);

            return await _repository.SaveChangesAsync();
        }
    }
}
