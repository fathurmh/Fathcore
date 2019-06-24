using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Infrastructure.Pagination;

namespace Fathcore.EntityFramework
{
    public partial class CachedRepository<TEntity> : IAsyncCachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        /// <summary>
        /// Asynchronously select all entities. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList));
            return await _cacheManager.GetAsync(key, () => _repository.SelectListAsync(cancellationToken));
        }

        /// <summary>
        /// Asynchronously select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), navigationProperty.Name);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p.Name).Select(p => p.Name)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(string navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), navigationProperty);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, navigationProperty.Name);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p.Name).Select(p => p.Name)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, navigationProperty);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), string.Join(".", navigationProperties.OrderBy(p => p)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(pagedInput, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), navigationProperty.Name);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(pagedInput, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), string.Join(".", navigationProperties.OrderBy(p => p.Name).Select(p => p.Name)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(pagedInput, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), navigationProperty);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(pagedInput, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), string.Join(".", navigationProperties.OrderBy(p => p)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(pagedInput, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, pagedInput, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), navigationProperty.Name);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, pagedInput, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), string.Join(".", navigationProperties.OrderBy(p => p.Name).Select(p => p.Name)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, pagedInput, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), navigationProperty);
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, pagedInput, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(SelectList), predicate.Name, pagedInput.PageIndex, pagedInput.PageSize, string.Join(".", pagedInput.PageSorts.OrderBy(p => p.KeySelector)), string.Join(".", navigationProperties.OrderBy(p => p)));
            return await _cacheManager.Get(key, () => _repository.SelectListAsync(predicate, pagedInput, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name);
            return await _cacheManager.Get(key, () => _repository.SelectAsync(predicate, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, navigationProperty.Name);
            return await _cacheManager.Get(key, () => _repository.SelectAsync(predicate, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, string.Join(".", navigationProperties.OrderBy(p => p.Name).Select(p => p.Name)));
            return await _cacheManager.Get(key, () => _repository.SelectAsync(predicate, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, navigationProperty);
            return await _cacheManager.Get(key, () => _repository.SelectAsync(predicate, navigationProperty, cancellationToken));
        }

        /// <summary>
        /// Asynchronously select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), predicate.Name, string.Join(".", navigationProperties.OrderBy(p => p)));
            return await _cacheManager.Get(key, () => _repository.SelectAsync(predicate, navigationProperties, cancellationToken));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(params object[] keyValues)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), keyValues.ToString());
            return await _cacheManager.Get(key, () => _repository.SelectAsync(keyValues));
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var key = string.Join(".", _cachePattern, nameof(Select), keyValues.ToString());
            return await _cacheManager.Get(key, () => _repository.SelectAsync(keyValues, cancellationToken));
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await _repository.InsertAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return await _repository.InsertAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await _repository.UpdateAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return await _repository.UpdateAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task DeleteAsync(params object[] keyValues)
        {
            await _repository.DeleteAsync(keyValues);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be deleted.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(keyValues, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Asynchronously change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity by this entry.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this cache to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cacheManager.RemoveByPattern(_cachePattern);
            return await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
