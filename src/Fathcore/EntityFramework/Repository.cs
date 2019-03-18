using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Represents the generic repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDbContext _context;
        private readonly DbSet<TEntity> _entities;

        /// <summary>
        /// Gets or sets the tracking behavior for LINQ queries run against the context. Disabling change tracking is useful for read-only scenarios because it avoids the overhead of setting up change tracking for each entity instance.
        /// You should not disable change tracking if you want to manipulate entity instances and persist those changes to the database using <see cref="DbContext.SaveChanges()"/>.
        /// The default value is <see cref="QueryTrackingBehavior.TrackAll"/>. This means the change tracker will keep track of changes for all entities that are returned from a LINQ query.
        /// </summary>
        private QueryTrackingBehavior TrackingBehavior { get; set; } = QueryTrackingBehavior.TrackAll;

        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <returns>A new query where the result set will be tracked by the context.</returns>
        private IQueryable<TEntity> TableTracking => _entities.AsTracking();

        /// <summary>
        /// Returns a new query where the change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and <see cref="DbContext.SaveChanges()"/> will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <returns>A new query where the result set will not be tracked by the context.</returns>

        private IQueryable<TEntity> TableNoTracking => _entities.AsNoTracking();

        /// <summary>
        /// Returns a new repository where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <return>A new repository where the result set will be tracked by the context.</return>
        public IRepository<TEntity> AsTracking()
        {
            TrackingBehavior = QueryTrackingBehavior.TrackAll;
            return this;
        }

        /// <summary>
        /// Returns a new repository where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <return>A new repository where the result set will not be tracked by the context.</return>
        public IRepository<TEntity> AsNoTracking()
        {
            TrackingBehavior = QueryTrackingBehavior.NoTracking;
            return this;
        }

        /// <summary>
        /// Returns a new query where the change tracker will keep track of changes for all entities that are returned.
        /// Any modification to the entity instances will be detected and persisted to the database during <see cref="DbContext.SaveChanges()"/>.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <value>A new query where the result set will be tracked by the context.</value>
        public IQueryable<TEntity> Table => TrackingBehavior == QueryTrackingBehavior.TrackAll ? TableTracking : TableNoTracking;

        /// <summary>
        /// Returns a new query where the query ignoring the filters and change tracker will not track any of the entities that are returned. If the entity instances are modified, this will not be detected by the change tracker and <see cref="DbContext.SaveChanges()"/> will not persist those changes to the database.
        /// The default tracking behavior for queries can be controlled by <see cref="QueryTrackingBehavior"/>.
        /// </summary>
        /// <returns>A new query where the result set ignore query filter and will not be tracked by the context.</returns>
        public IQueryable<TEntity> TableNoFilters => Table.IgnoreQueryFilters();

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
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
        public virtual IEnumerable<TEntity> SelectList()
        {
            return Table.ToList();
        }

        /// <summary>
        /// Select all entities. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync()
        {
            return await Table.ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return Table.Include(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return await Table.Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(params string[] navigationProperties)
        {
            return Table.Include(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(params string[] navigationProperties)
        {
            return await Table.Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate).ToList();
        }

        /// <summary>
        /// Select all entities and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// If no entity is found, then zero collection is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return Table.Where(predicate).Include(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return Table.Where(predicate).Include(navigationProperties).ToList();
        }

        /// <summary>
        /// Select all entities with the given navigation property values and filters a sequence of values based on a predicate. If the entities is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entities, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entities found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or zero collection.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return await Table.Where(predicate).Include(navigationProperties).ToListAsync();
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Select an entity and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return Table.Where(predicate).Include(navigationProperties).FirstOrDefault();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return await Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return Table.Where(predicate).Include(navigationProperties).FirstOrDefault();
        }

        /// <summary>
        /// Select an entity with the given navigation property values and filters a sequence of values based on a predicate. If the entity is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for the entity, if found, is attached to the context and returned.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call with comma separated.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included ("Property1").</param>
        /// <returns>A task that represents the asynchronous operation. The entity found that contains elements from the input sequence that satisfy the condition specified by predicate predicate, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            return await Table.Where(predicate).Include(navigationProperties).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual TEntity Select(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            IEnumerable<INavigation> navigationProperties = default;
            TEntity entity = default;

            if (_context is DbContext dbContext)
                navigationProperties = dbContext.Model.FindEntityType(typeof(TEntity)).GetNavigations();

            if (navigationProperties == null)
            {
                entity = _entities.Find(keyValue);
            }
            else
            {
                var navigationPropertyNames = new List<string>();

                foreach (INavigation navigationProperty in navigationProperties)
                {
                    if (navigationPropertyNames.Contains(navigationProperty.Name))
                        continue;

                    navigationPropertyNames.Add(navigationProperty.Name);
                }

                entity = Select(prop => prop.Id.Equals(keyValue), navigationPropertyNames.ToArray());
            }

            if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                _context.Detach(entity);

            return entity;
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values is being tracked by the context, then it is returned immediately without making a request to the database.
        /// Otherwise, a query is made to the database for an entity with the given primary key values and this entity, if found, is attached to the context and returned.
        /// If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous operation. The entity found, or null.</returns>
        public virtual async Task<TEntity> SelectAsync(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            IEnumerable<INavigation> navigationProperties = default;
            TEntity entity = default;

            if (_context is DbContext dbContext)
                navigationProperties = dbContext.Model.FindEntityType(typeof(TEntity)).GetNavigations();

            if (navigationProperties == null)
            {
                entity = await _entities.FindAsync(keyValue);
            }
            else
            {
                var navigationPropertyNames = new List<string>();

                foreach (INavigation navigationProperty in navigationProperties)
                {
                    if (navigationPropertyNames.Contains(navigationProperty.Name))
                        continue;

                    navigationPropertyNames.Add(navigationProperty.Name);
                }

                entity = await SelectAsync(prop => prop.Id.Equals(keyValue), navigationPropertyNames.ToArray());
            }

            if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                _context.Detach(entity);

            return entity;
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

            entity = _entities.Add(entity).Entity;

            return entity;
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

            entity = (await _entities.AddAsync(entity)).Entity;

            return entity;
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

            _entities.AddRange(entities);

            return entities;
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
                throw new ArgumentNullException(nameof(entity));

            entity = _entities.Update(entity).Entity;

            return entity;
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Modified state such that it will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entity being tracked by this entry.</returns>
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity = _entities.Update(entity).Entity;

            return Task.FromResult(entity);
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
                throw new ArgumentNullException(nameof(entities));

            _entities.UpdateRange(entities);

            return entities;
        }

        /// <summary>
        /// Begins tracking the given entities in the EntityState.Modified state such that they will be updated in the database when SaveChanges is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities that are not already being tracked by the context.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>A task that represents the asynchronous operation. Returns the entities being tracked by this entry.</returns>
        public virtual Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            _entities.UpdateRange(entities);

            return Task.FromResult(entities);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
        public virtual void Delete(object keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException(nameof(keyValue));

            var entity = Select(keyValue);
            _entities.Remove(entity);
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

            var entity = await SelectAsync(keyValue);
            _entities.Remove(entity);
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that it will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Remove(entity);
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

            await Task.Run(() => _entities.Remove(entity));
        }

        /// <summary>
        /// Begins tracking the given entity in the EntityState.Deleted state such that they will be removed from the database when SaveChanges is called.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                throw new ArgumentNullException(nameof(entities));

            _entities.RemoveRange(entities);
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
                var count = _context.SaveChanges();

                if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                {
                    var currentEntries = _context.ChangeTracker.Entries<TEntity>().ToList();
                    _context.DetachRange(currentEntries.Select(p => p.Entity));
                }

                return count;
            }
            catch (DbUpdateException exception)
            {
                string errorText = _context.RollbackEntityChanges(exception);
                throw new Exception(errorText, exception);
            }
            catch
            {
                throw;
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
                var count = await _context.SaveChangesAsync();

                if (TrackingBehavior == QueryTrackingBehavior.NoTracking)
                {
                    var currentEntries = _context.ChangeTracker.Entries<TEntity>().ToList();
                    _context.DetachRange(currentEntries.Select(p => p.Entity));
                }

                return count;
            }
            catch (DbUpdateException exception)
            {
                string errorText = await _context.RollbackEntityChangesAsync(exception);
                throw new Exception(errorText, exception);
            }
            catch
            {
                throw;
            }
        }
    }
}
