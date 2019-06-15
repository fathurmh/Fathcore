using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fathcore.Extensions;
using Fathcore.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework.Extensions
{
    /// <summary>
    /// Extension methods for Repository.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository, params Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository, params string[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IEnumerable<TEntity>> SelectListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select paged entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, IPaginationData<TEntity> paginationData)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, IPaginationData<TEntity> paginationData, params Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Include(navigationProperties).ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, IPaginationData<TEntity> paginationData, params string[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Include(navigationProperties).ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select paged entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData, params Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select paged entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public static async Task<IPagedList<TEntity>> PagedListAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData, params string[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).ToPagedListAsync(paginationData);
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public static async Task<TEntity> SelectAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public static async Task<TEntity> SelectAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public static async Task<TEntity> SelectAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return await repository.Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public static async Task<TEntity> SelectAsync<TEntity>(this IRepository<TEntity> repository, object keyValue)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            TEntity entity = await repository.DbContext.Set<TEntity>().FindAsync(keyValue);
            if (repository.DbContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking)
                repository.DbContext.Detach(entity);

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public static async Task<TEntity> InsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity = (await repository.DbContext.Set<TEntity>().AddAsync(entity)).Entity;

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are not already being tracked,
        /// in the EntityState.Added state such that they will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public static async Task<IEnumerable<TEntity>> InsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await repository.DbContext.Set<TEntity>().AddRangeAsync(entities);

            return entities;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public static Task<TEntity> UpdateAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity = repository.DbContext.Set<TEntity>().Update(entity).Entity;

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The entities to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public static Task<IEnumerable<TEntity>> UpdateAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            repository.DbContext.Set<TEntity>().UpdateRange(entities);

            return Task.FromResult(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task DeleteAsync<TEntity>(this IRepository<TEntity> repository, object keyValue)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            var entity = await repository.SelectAsync(keyValue);
            repository.DbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task DeleteAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Task.Run(() => repository.DbContext.Set<TEntity>().Remove(entity));
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The entities to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task DeleteAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            await Task.Run(() => repository.DbContext.Set<TEntity>().RemoveRange(entities));
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <typeparam name="TEntity">The class inherits from <see cref="BaseEntity{TEntity}"/>.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public static async Task<int> SaveChangesAsync<TEntity>(this IRepository<TEntity> repository)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            try
            {
                var count = await repository.DbContext.SaveChangesAsync();

                if (repository.DbContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking)
                {
                    var currentEntries = repository.DbContext.GetCurrentEntries();
                    repository.DbContext.DetachRange(currentEntries.Select(p => (TEntity)p.Entity));
                }

                return count;
            }
            catch (DbUpdateException)
            {
                await repository.DbContext.RollbackEntityChangesAsync();
                throw;
            }
        }
    }
}
