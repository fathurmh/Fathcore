using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Extensions;
using Fathcore.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework
{
    public partial class Repository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(CancellationToken cancellationToken = default)
        {
            return await Table.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperty).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperties).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperty">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(string navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperty).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperties).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Select paged entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            return await Table.ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperty).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperties).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperty).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Include(navigationProperties).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToPagedListAsync(pagedInput, cancellationToken);
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperty).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            return await Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(params object[] keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            TEntity entity = await _entities.FindAsync(keyValue);
            if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                _dbContext.Detach(entity);

            return entity;
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(object[] keyValue, CancellationToken cancellationToken = default)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            TEntity entity = await _entities.FindAsync(keyValue, cancellationToken);
            if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                _dbContext.Detach(entity);

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _entities.AddAsync(entity, cancellationToken);

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await _entities.AddRangeAsync(entities, cancellationToken);

            return entities;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Task.Run(() => _entities.Update(entity), cancellationToken);

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await Task.Run(() => _entities.UpdateRange(entities), cancellationToken);

            return entities;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be deleted.</param>
        public virtual async Task DeleteAsync(params object[] keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            var entity = await SelectAsync(keyValue);
            _entities.Remove(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be deleted.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(object[] keyValue, CancellationToken cancellationToken = default)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            var entity = await SelectAsync(keyValue, cancellationToken);
            _entities.Remove(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Task.Run(() => _entities.Remove(entity), cancellationToken);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await Task.Run(() => _entities.RemoveRange(entities), cancellationToken);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _dbContext.SaveChangesAsync(cancellationToken);

                if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                {
                    var currentEntries = _dbContext.GetCurrentEntries();
                    _dbContext.DetachRange(currentEntries.Select(p => (TEntity)p.Entity));
                }

                return count;
            }
            catch (DbUpdateException)
            {
                await _dbContext.RollbackEntityChangesAsync(cancellationToken);
                throw;
            }
        }
    }
}
