using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fathcore.EntityFramework.AuditTrail
{
    /// <summary>
    /// Represents an audit handler.
    /// </summary>
    public class AuditHandler : IAuditHandler
    {
        public const string DefaultName = "Anonymous";
        //private readonly IPrincipal _principal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly string _currentUsername;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHandler"/> class.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public AuditHandler(IHttpContextAccessor httpContextAccessor)
        {
            //_currentUsername = httpContextAccessor?.HttpContext?.User?.Identity.Name ?? DefaultName;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        public virtual void Handle(BaseDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            var auditEntries = dbContext.GetCurrentEntries().ToList();
            auditEntries.ForEach(entry => AuditEntity(entry));
        }

        /// <summary>
        /// Asynchronously applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task HandleAsync(BaseDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            var auditEntries = dbContext.GetCurrentEntries().ToList();
            await auditEntries.ForEachAsync(entry => AuditEntityEntryAsync(entry));
        }

        /// <summary>
        /// Get the dependent entity entry.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being looking for the dependent.</param>
        /// <returns>An array of dependent entity entry.</returns>
        protected EntityEntry[] GetDependentEntity(EntityEntry entry)
        {
            var dependentEntries = new List<EntityEntry>();

            foreach (var navigationEntry in entry.Navigations.Where(n => !n.Metadata.IsDependentToPrincipal()))
            {
                if (navigationEntry is CollectionEntry collectionEntry)
                {
                    foreach (var dependent in collectionEntry.CurrentValue)
                        dependentEntries.Add(entry.Context.Entry(dependent));
                }
                else
                {
                    var dependent = navigationEntry.CurrentValue;

                    if (dependent != null)
                        dependentEntries.Add(entry.Context.Entry(dependent));
                }
            }

            return dependentEntries.ToArray();
        }

        /// <summary>
        /// Audit the current auditable entity in <see cref="EntityState.Added"/> state with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        /// <param name="dateTime">The current <see cref="DateTime"/>.</param>
        protected virtual void AuditAddedEntity(EntityEntry entry, DateTime dateTime)
        {
            Type auditEntryType = entry.Entity.GetType();
            if (typeof(IAuditable).IsAssignableFrom(auditEntryType))
            {
                entry.CurrentValues[nameof(IAuditable.CreatedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity.Name ?? DefaultName;
                entry.CurrentValues[nameof(IAuditable.CreatedTime)] = dateTime;
                entry.State = EntityState.Added;
            }
        }

        /// <summary>
        /// Audit the current auditable entity in <see cref="EntityState.Modified"/> state with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        /// <param name="dateTime">The current <see cref="DateTime"/>.</param>
        protected virtual void AuditModifiedEntity(EntityEntry entry, DateTime dateTime)
        {
            Type auditEntryType = entry.Entity.GetType();

            if (typeof(IAuditable).IsAssignableFrom(auditEntryType))
            {
                entry.CurrentValues[nameof(IAuditable.ModifiedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity.Name ?? DefaultName;
                entry.CurrentValues[nameof(IAuditable.ModifiedTime)] = dateTime;
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Audit the current auditable entity in <see cref="EntityState.Deleted"/> state with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        /// <param name="dateTime">The current <see cref="DateTime"/>.</param>
        protected virtual void AuditDeletedEntity(EntityEntry entry, DateTime dateTime)
        {
            Type auditEntryType = entry.Entity.GetType();

            if (typeof(ISoftDeletable).IsAssignableFrom(auditEntryType))
            {
                entry.State = EntityState.Unchanged;
                entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;
                entry.CurrentValues[nameof(ISoftDeletable.DeletedTime)] = dateTime;
                AuditModifiedEntity(entry, dateTime);

                var dependentEntries = GetDependentEntity(entry);

                foreach (var dependentEntry in dependentEntries)
                {
                    AuditDeletedEntity(dependentEntry, dateTime);
                }
            }
        }

        /// <summary>
        /// Audit the current auditable entity with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        protected virtual void AuditEntity(EntityEntry entry)
        {
            var dateTime = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    AuditAddedEntity(entry, dateTime);
                    break;
                case EntityState.Modified:
                    AuditModifiedEntity(entry, dateTime);
                    break;
                case EntityState.Deleted:
                    AuditDeletedEntity(entry, dateTime);
                    break;
            }
        }

        /// <summary>
        /// Asynchronously audit the current auditable entity with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task AuditEntityEntryAsync(EntityEntry entry)
        {
            return Task.Run(() => AuditEntity(entry));
        }
    }
}
