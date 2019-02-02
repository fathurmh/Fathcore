using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Fathcore.Data.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Represents a db context extensions class
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Migrate database context that inherits from DbContext or implements from IDbContext
        /// </summary>
        /// <param name="host"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static async Task DatabaseMigrateAsync(this IWebHost host, Assembly assembly)
        {
            using (var scope = host.Services.CreateScope())
            {
                var contextTypes = new List<Type>();
                var assemblyName = assembly.FullName.Split('.')[0];
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(prop => prop.FullName.Contains(assemblyName)).ToList()
                    .ForEach(item =>
                        contextTypes.AddRange(item.GetTypes()
                            .Where(type => (type.BaseType == typeof(DbContext) || type.GetInterfaces().Contains(typeof(IDbContext))))));
                
                foreach (var contextType in contextTypes)
                {
                    try
                    {
                        var context = (DbContext)scope.ServiceProvider.GetRequiredService(contextType);
                        await context.Database.MigrateAsync();
                    }
                    catch
                    {
                        // Logging here soon
                    }
                }
            }
        }
        
        /// <summary>
        /// Audit db context
        /// </summary>
        /// <param name="context">DbContext</param>
        public static void Audit(this IDbContext context)
        {
            string currentUsername = EngineContext.Current.Principal.Identity.Name;
            IEnumerable<EntityEntry> auditEntries = context.GetCurrentEntries();

            try
            {
                foreach (var auditEntry in auditEntries)
                {
                    DateTime dateTimeNow = DateTime.Now;

                    switch(auditEntry.State)
                    {
                        case EntityState.Added:
                        {
                            auditEntry.CurrentValues[nameof(IAuditable.CreatedBy)] = currentUsername;
                            auditEntry.CurrentValues[nameof(IAuditable.CreatedTime)] = dateTimeNow;
                        }
                        break;
                        case EntityState.Modified:
                        {
                            auditEntry.WriteUserModifier(currentUsername, dateTimeNow);
                        }
                        break;
                        case EntityState.Deleted:
                        {
                            if (typeof(ISoftDeletable).IsAssignableFrom(auditEntry.Entity.GetType()))
                            {
                                var deletedKeyProperty = auditEntry.OriginalValues.Properties.Where(prop => prop.Name.Equals(nameof(ISoftDeletable.IsDeleted))).SingleOrDefault();

                                auditEntry.CurrentValues[deletedKeyProperty] = true;
                                auditEntry.CurrentValues[nameof(ISoftDeletable.DeletedTime)] = dateTimeNow;
                                auditEntry.WriteUserModifier(currentUsername, dateTimeNow);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Rollback of entity changes and return full error message asynchronously
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception">Exception</param>
        /// <returns>Error message</returns>
        public static string RollbackEntityChanges(this IDbContext context, DbUpdateException exception)
        {
            context.RollbackEntity();
            context.SaveChanges();
            return exception.ToString();
        }

        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception">Exception</param>
        /// <returns>Error message</returns>
        public static async Task<string> RollbackEntityChangesAsync(this IDbContext context, DbUpdateException exception)
        {
            context.RollbackEntity();
            await context.SaveChangesAsync();
            return exception.ToString();
        }
        
        /// <summary>
        /// Get current entity entries
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<EntityEntry> GetCurrentEntries(this IDbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted);
        }
        
        /// <summary>
        /// Specifies related entities to include in the query results.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call to with comma (,).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="entities">The source entities query</param>
        /// <param name="navigationProperties">A lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A new query with the related data included.</returns>
        public static IQueryable<TEntity> IncludeProperties<TEntity>(this IQueryable<TEntity> entities, Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : BaseEntity
        {
            foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
            {
                entities = entities.Include(navigationProperty);
            }
            return entities;
        }

        /// <summary>
        /// Specifies related entities to include in the query results.
        /// The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// If you wish to include additional types based on the navigation properties of the type being included, then chain a call to with comma (,).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="entities">The source entities query</param>
        /// <param name="navigationProperties">A string representing the navigation property to be included (Property1).</param>
        /// <returns>A new query with the related data included.</returns>
        public static IQueryable<TEntity> IncludeProperties<TEntity>(this IQueryable<TEntity> entities, string[] navigationProperties)
            where TEntity : BaseEntity
        {
            foreach (string navigationProperty in navigationProperties)
            {
                entities = entities.Include(navigationProperty);
            }
            return entities;
        }
        
        private static EntityEntry WriteUserModifier(this EntityEntry entry, string currentUsername, DateTime now)
        {
            Type auditEntryType = entry.Entity.GetType();
            if (typeof(IAuditable).IsAssignableFrom(auditEntryType))
            {
                entry.CurrentValues[nameof(IAuditable.ModifiedBy)] = currentUsername;
                entry.CurrentValues[nameof(IAuditable.ModifiedTime)] = now;
                entry.State = EntityState.Modified;
            }
            return entry;
        }

        private static void RollbackEntity(this IDbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            entries.ForEach(entry =>
            {
                try
                {
                    entry.State = EntityState.Unchanged;
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            });
        }
    }
}
