using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fathcore.Data.Abstractions;
using Fathcore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.Data
{
    /// <summary>
    /// Represents the generic repository pattern.
    /// This isn’t useful with Entity Framework.
    /// EF Core already implements a Rep/UoW pattern, so layering another Rep/UoW pattern on top of EF Core isn’t helpful.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private IDbContext _context;
        private DbSet<TEntity> _entities;

        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during Microsoft.EntityFrameworkCore.DbContext.SaveChanges .
        /// The default tracking behavior for queries can be controlled by Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.QueryTrackingBehavior .
        /// </summary>
        /// <value>A new query where the result set will be tracked by the context.</value>
        public virtual IQueryable<TEntity> Table => _entities.AsTracking();

        /// <summary>
        /// Returns a new query where the change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and Microsoft.EntityFrameworkCore.DbContext.SaveChanges will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.QueryTrackingBehavior .
        /// </summary>
        /// <returns>A new query where the result set will not be tracked by the context.</returns>
        public virtual IQueryable<TEntity> TableNoTracking => _entities.AsNoTracking();

        public Repository(IDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select()
        {
            return TableNoTracking.ToList();
        }

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync()
        {
            return await TableNoTracking.ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return TableNoTracking.IncludeProperties(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return await TableNoTracking.IncludeProperties(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select(params string[] navigationProperties)
        {
            return TableNoTracking.IncludeProperties(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(params string[] navigationProperties)
        {
            return await TableNoTracking.IncludeProperties(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate).ToList();
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return Table.Where(predicate).IncludeProperties(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return await Table.Where(predicate).IncludeProperties(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return Table.Where(predicate).IncludeProperties(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">The values of the navigation property for the entity.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return await Table.Where(predicate).IncludeProperties(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual TEntity Select(params object[] keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            return _entities.Find(keyValues);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(params object[] keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            return await _entities.FindAsync(keyValues);
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
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return _entities.Add(entity).Entity;
        }

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return (await _entities.AddAsync(entity)).Entity;
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
            {
                throw new ArgumentNullException(nameof(entities));
            }

            _entities.AddRange(entities);
            return entities;
        }

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await _entities.AddRangeAsync(entities);
            return entities;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return _entities.Update(entity).Entity;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Returns the entity being tracked by this entry.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Task.FromResult(_entities.Update(entity).Entity);
        }
        
        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            _entities.UpdateRange(entities);
            return entities;
        }
        
        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>Returns the entities being tracked by this entry.</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            
            await Task.Run(() => _entities.UpdateRange(entities));
            return await Task.FromResult(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        public virtual void Delete(params object[] keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            _entities.Remove(Select(keyValues));
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        public virtual async Task DeleteAsync(params object[] keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            await Task.FromResult(_entities.Remove(await SelectAsync(keyValues)));
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _entities.Remove(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Task.FromResult(_entities.Remove(entity));
        }
        
        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            _entities.RemoveRange(entities);
        }
        
        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await Task.Run(() => _entities.RemoveRange(entities));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public virtual int SaveChanges()
        {
            try
            {
                _context.Audit();
                return _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                string errorText = _context.RollbackEntityChanges(exception);
                throw new Exception(errorText, exception);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {
                _context.Audit();
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                string errorText = await _context.RollbackEntityChangesAsync(exception);
                throw new Exception(errorText, exception);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
