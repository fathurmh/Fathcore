using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fathcore.EntityFramework.Audit
{
    /// <summary>
    /// Represents an audit handler.
    /// </summary>
    internal class AuditHandler : IAuditHandler
    {
        private readonly IPrincipal _principal;

        public AuditHandler(IHttpContextAccessor httpContextAccessor)
        {
            _principal = httpContextAccessor?.HttpContext?.User ?? new GenericPrincipal(new GenericIdentity("Anonymous"), null);
        }

        /// <summary>
        /// Audit the current auditable entity with specified values.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> being audit.</param>
        /// <param name="dateTimeNow">The current <see cref="DateTime"/>.</param>
        private void AuditEntityEntry(EntityEntry entry, DateTime dateTimeNow)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            Type auditEntryType = entry.Entity.GetType();

            switch (entry.State)
            {
                case EntityState.Added:
                {
                    if (typeof(IAuditable).IsAssignableFrom(auditEntryType))
                    {
                        entry.CurrentValues[nameof(IAuditable.CreatedBy)] = _principal.Identity.Name;
                        entry.CurrentValues[nameof(IAuditable.CreatedTime)] = dateTimeNow;
                    }
                }
                break;
                case EntityState.Modified:
                {
                    if (typeof(IAuditable).IsAssignableFrom(auditEntryType))
                    {
                        entry.CurrentValues[nameof(IAuditable.ModifiedBy)] = _principal.Identity.Name;
                        entry.CurrentValues[nameof(IAuditable.ModifiedTime)] = dateTimeNow;
                    }
                }
                break;
                case EntityState.Deleted:
                {
                    if (typeof(ISoftDeletable).IsAssignableFrom(auditEntryType))
                    {
                        entry.State = EntityState.Unchanged;
                        entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;
                        entry.CurrentValues[nameof(ISoftDeletable.DeletedTime)] = dateTimeNow;
                        entry.State = EntityState.Modified;

                        foreach (var navigationEntry in entry.Navigations.Where(n => !n.Metadata.IsDependentToPrincipal()))
                        {
                            if (navigationEntry is CollectionEntry collectionEntry)
                            {
                                foreach (var dependent in collectionEntry.CurrentValue)
                                {
                                    var dependentEntry = entry.Context.Entry(dependent);
                                    dependentEntry.State = EntityState.Deleted;
                                    AuditEntityEntry(dependentEntry, dateTimeNow);
                                }
                            }
                            else
                            {
                                var dependent = navigationEntry.CurrentValue;
                                if (dependent != null)
                                {
                                    var dependentEntry = entry.Context.Entry(dependent);
                                    dependentEntry.State = EntityState.Deleted;
                                    AuditEntityEntry(dependentEntry, dateTimeNow);
                                }
                            }
                        }

                        AuditEntityEntry(entry, dateTimeNow);
                    }
                }
                break;
            }
        }

        /// <summary>
        /// Applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        public void Handle(BaseDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            try
            {
                var auditEntries = dbContext.GetCurrentEntries().ToList();
                auditEntries.ForEach(entry => AuditEntityEntry(entry, DateTime.Now));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Asynchronously applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(BaseDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            try
            {
                var auditEntries = dbContext.GetCurrentEntries().ToList();
                await auditEntries.ForEachAsync(async entry => await Task.Run(() => AuditEntityEntry(entry, DateTime.Now)).ConfigureAwait(false));
            }
            catch
            {
                throw;
            }
        }
    }
}
