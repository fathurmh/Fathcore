using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fathcore.Extensions;
using Fathcore.Infrastructure.Caching;
using Fathcore.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the class for generic cached repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public partial class CachedRepository<TEntity> : ICachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly string _cachePattern;

        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <value>A new query where the result set will be tracked by the context.</value>
        public IQueryable<TEntity> Table => _repository.Table;

        /// <summary>
        /// Returns a new query where the query ignoring the filters and change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and <see cref="DbContext.SaveChanges()"/> will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <returns>A new query where the result set ignore query filter and will not be tracked by the context.</returns>
        public IQueryable<TEntity> TableNoFilters => _repository.TableNoFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="repository">An instance of <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="cacheManager">An instance of <see cref="IStaticCacheManager"/>.</param>
        public CachedRepository(IRepository<TEntity> repository, IStaticCacheManager cacheManager)
        {
            _repository = repository.AsNoTracking();
            _cacheManager = cacheManager;
            _cachePattern = string.Join(".", _cacheManager.CacheSetting?.CachePrefix ?? string.Empty, typeof(TEntity).Name);
        }

        /// <summary>
        /// Select all entities. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList()
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList));
            return _cacheManager.Get(key, () => _repository.SelectList());
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, object>> navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), navigationProperty.GetBodyString());
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperty));
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p.GetBodyString()).Select(p => p.GetBodyString())));
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperties));
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(string navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), navigationProperty);
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperty));
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(IEnumerable<string> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p)));
            return _cacheManager.Get(key, () => _repository.SelectList(navigationProperties));
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString());
            return _cacheManager.Get(key, () => _repository.SelectList(predicate));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), navigationProperty.GetBodyString());
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperty));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), string.Join(".", navigationProperties.OrderBy(p => p.GetBodyString()).Select(p => p.GetBodyString())));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperties));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, string navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), navigationProperty);
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperty));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), string.Join(".", navigationProperties.OrderBy(p => p)));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, navigationProperties));
        }

        /// <summary>
        /// Select paged entities. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()));
            return _cacheManager.Get(key, () => _repository.SelectList(pagedInput));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), navigationProperty.GetBodyString());
            return _cacheManager.Get(key, () => _repository.SelectList(pagedInput, navigationProperty));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), string.Join(".", navigationProperties.OrderBy(p => p.GetBodyString()).Select(p => p.GetBodyString())));
            return _cacheManager.Get(key, () => _repository.SelectList(pagedInput, navigationProperties));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, string navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), navigationProperty);
            return _cacheManager.Get(key, () => _repository.SelectList(pagedInput, navigationProperty));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), string.Join(".", navigationProperties.OrderBy(p => p)));
            return _cacheManager.Get(key, () => _repository.SelectList(pagedInput, navigationProperties));
        }

        /// <summary>
        /// Select paged entities and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, pagedInput));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), navigationProperty.GetBodyString());
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, pagedInput, navigationProperty));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), string.Join(".", navigationProperties.OrderBy(p => p.GetBodyString()).Select(p => p.GetBodyString())));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, pagedInput, navigationProperties));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), navigationProperty);
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, pagedInput, navigationProperty));
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.GetBodyString(), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector).ToString()), string.Join(".", navigationProperties.OrderBy(p => p)));
            return _cacheManager.Get(key, () => _repository.SelectList(predicate, pagedInput, navigationProperties));
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.GetBodyString());
            return _cacheManager.Get(key, () => _repository.Select(predicate));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.GetBodyString(), navigationProperty.GetBodyString());
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperty));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.GetBodyString(), string.Join(".", navigationProperties.OrderBy(p => p.GetBodyString()).Select(p => p.GetBodyString())));
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperties));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, string navigationProperty)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.GetBodyString(), navigationProperty);
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperty));
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.GetBodyString(), string.Join(".", navigationProperties.OrderBy(p => p)));
            return _cacheManager.Get(key, () => _repository.Select(predicate, navigationProperties));
        }

        /// <summary>
        /// Finds an entity with the given primary key value. If an entity with the given primary key value is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key value and this entity.
        /// </summary>
        /// <param name="keyValue">The value of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual TEntity Select(object keyValue)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), keyValue.ToString());
            return _cacheManager.Get(key, () => _repository.Select(keyValue));
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Added state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual TEntity Insert(TEntity entity)
        {
            return _repository.Insert(entity);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Added state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            return _repository.Insert(entities);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Modified state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual TEntity Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Modified state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            return _repository.Update(entities);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be deleted.</param>
        public virtual void Delete(object keyValue)
        {
            _repository.Delete(keyValue);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be deleted.</param>
        public virtual void Delete(IEnumerable<object> keyValues)
        {
            _repository.Delete(keyValues);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            _repository.Delete(entity);
        }

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
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
    }
}
