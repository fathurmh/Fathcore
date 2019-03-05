using System.Threading.Tasks;

namespace Fathcore.EntityFramework.Audit
{
    /// <summary>
    /// Represents an audit handler.
    /// </summary>
    public partial interface IAuditHandler
    {
        /// <summary>
        /// Applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        void Handle(BaseDbContext dbContext);

        /// <summary>
        /// Asynchronously applies any audit entries for the context to the database.
        /// </summary>
        /// <param name="dbContext">The <see cref="BaseDbContext"/> being audit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleAsync(BaseDbContext dbContext);
    }
}
