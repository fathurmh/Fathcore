using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fathcore.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for generic cached repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public partial interface ICachedRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <value>A new query where the result set will be tracked by the context.</value>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// Returns a new query where the query ignoring the filters and change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and <see cref="DbContext.SaveChanges()"/> will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <returns>A new query where the result set ignore query filter and will not be tracked by the context.</returns>
        IQueryable<TEntity> TableNoFilters { get; }

        /// <summary>
        /// Select all entities. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList();

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList(string navigationProperty);

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> SelectList(IEnumerable<string> navigationProperties);

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, string navigationProperty);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties);

        /// <summary>
        /// Select paged entities. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <returns>The entities found, or zero collection.</returns>
        IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput);

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties);

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, string navigationProperty);

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties);

        /// <summary>
        /// Select paged entities and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput);

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties);

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty);

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pagedInput">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties);

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperty">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate, string navigationProperty);

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is exists in cache, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties);

        /// <summary>
        /// Finds an entity with the given primary key value. If an entity with the given primary key value is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key value and this entity.
        /// </summary>
        /// <param name="keyValue">The value of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        TEntity Select(object keyValue);

        /// <summary>
        /// Change the given entity state to the EntityState.Added state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Change the given entity state to the EntityState.Added state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Change the given entity state to the EntityState.Modified state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Change the given entity state to the EntityState.Modified state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be deleted.</param>
        void Delete(object keyValue);

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be deleted.</param>
        void Delete(IEnumerable<object> keyValues);

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Change the given entity state to the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();
    }
}
