using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fathcore.Data.Abstractions
{
    /// <summary>
    /// Represents the generic repository pattern.
    /// This isn’t useful with Entity Framework.
    /// EF Core already implements a Rep/UoW pattern, so layering another Rep/UoW pattern on top of EF Core isn’t helpful.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during Microsoft.EntityFrameworkCore.DbContext.SaveChanges .
        /// The default tracking behavior for queries can be controlled by Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.QueryTrackingBehavior .
        /// </summary>
        /// <value>A new query where the result set will be tracked by the context.</value>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// Returns a new query where the change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and Microsoft.EntityFrameworkCore.DbContext.SaveChanges will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.QueryTrackingBehavior .
        /// </summary>
        /// <returns>A new query where the result set will not be tracked by the context.</returns>
        IQueryable<TEntity> TableNoTracking { get; }

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> Select();

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync();
        
        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> Select(params Expression<Func<TEntity, object>>[] navigationProperties);
        
        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync(params Expression<Func<TEntity, object>>[] navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        IEnumerable<TEntity> Select(params string[] navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync(params string[] navigationProperties);

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties);

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        TEntity Select(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TEntity> SelectAsync(params object[] keyValues);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        void Delete(params object[] keyValues);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        Task DeleteAsync(params object[] keyValues);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        Task DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
    }
}
