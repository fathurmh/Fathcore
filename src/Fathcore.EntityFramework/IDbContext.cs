using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Represents database context.
    /// </summary>
    public partial interface IDbContext : IDisposable
    {
        /// <summary>
        /// Provides access to information and operations for entity instances this context is tracking.
        /// </summary>
        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// Detach an entity is being tracked by a context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity is not being tracked.</typeparam>
        /// <param name="entity">The entity is not being tracked by the context.</param>
        void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Creates a LINQ query for the entity based on a raw SQL query.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="sql">The raw SQL query.</param>
        /// <param name="parameters">The values to be assigned to parameters.</param>
        /// <returns>Returns an IQueryable representing the raw SQL query.</returns>
        IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity;

        /// <summary>
        /// Executes the given SQL against the database.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="doNotEnsureTransaction">true - the transaction creation is not ensured; false - the transaction creation is ensured.</param>
        /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string.</param>
        /// <param name="parameters">Parameters to use with the SQL.</param>
        /// <returns>Returns the number of rows affected.</returns>
        int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// Generate a script to create all tables for the current model.
        /// </summary>
        /// <returns>Returns a SQL script.</returns>
        string GenerateCreateScript();

        /// <summary>
        /// Creates a LINQ query for the query type based on a raw SQL query.
        /// </summary>
        /// <typeparam name="TQuery">Query type.</typeparam>
        /// <param name="sql">The raw SQL query.</param>
        /// <returns>Returns an IQueryable representing the raw SQL query.</returns>
        IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class;

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>Returns the number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being operated on by this set.</typeparam>
        /// <returns>Returns a set for the given entity type.</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
    }
}
