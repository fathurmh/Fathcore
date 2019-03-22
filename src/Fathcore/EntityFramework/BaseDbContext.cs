using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.EntityFramework.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the base class of database context that represents a session with the database and can be used to query and save instances of your entities.
    /// </summary>
    public abstract class BaseDbContext : DbContext, IDbContext
    {
        private readonly IAuditHandler _auditHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbContext"/> class using the specified options.
        /// The <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/> method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public BaseDbContext(DbContextOptions options)
            : base(options)
        {
            _auditHandler = Engine.Current.Resolve<IAuditHandler>();
        }

        /// <summary>
        /// Further configuration the model.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsSqlite() || Database.IsInMemory())
            {
                var timestampProperties = modelBuilder.Model
                    .GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(byte[])
                        && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
                        && p.IsConcurrencyToken);

                foreach (var property in timestampProperties)
                {
                    property.SetValueConverter(new SqliteTimestampConverter());
                    property.Relational().DefaultValueSql = "CURRENT_TIMESTAMP";
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Detach an entity is being tracked by a context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity is not being tracked.</typeparam>
        /// <param name="entity">The entity is not being tracked by the context.</param>
        public void Detach<TEntity>(TEntity entity)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            EntityEntry<TEntity> entityEntry = Entry(entity);
            entityEntry.State = EntityState.Detached;
        }

        /// <summary>
        /// Detach entities are being tracked by a context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity is not being tracked.</typeparam>
        /// <param name="entities">The entities are not being tracked by the context.</param>
        public void DetachRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                Detach(entity);
        }

        /// <summary>
        /// Creates a LINQ query for the entity based on a raw SQL query.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="sql">The raw SQL query.</param>
        /// <param name="parameters">The values to be assigned to parameters.</param>
        /// <returns>Returns an IQueryable representing the raw SQL query.</returns>
        public virtual IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters)
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        /// <summary>
        /// Executes the given SQL against the database.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="ensureTransaction">true - the transaction creation is ensured; false - the transaction creation is not ensured.</param>
        /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string.</param>
        /// <param name="parameters">Parameters to use with the SQL.</param>
        /// <returns>Returns the number of rows affected.</returns>
        public virtual int ExecuteSqlCommand(RawSqlString sql, bool ensureTransaction, int? timeout, params object[] parameters)
        {
            var previousTimeout = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            var result = 0;
            if (ensureTransaction)
            {
                using (IDbContextTransaction transaction = Database.BeginTransaction())
                {
                    result = Database.ExecuteSqlCommand(sql, parameters);
                    transaction.Commit();
                }
            }
            else
            {
                result = Database.ExecuteSqlCommand(sql, parameters);
            }

            Database.SetCommandTimeout(previousTimeout);

            return result;
        }

        /// <summary>
        /// Generate a script to create all tables for the current model.
        /// </summary>
        /// <returns>Returns a SQL script.</returns>
        public virtual string GenerateCreateScript()
        {
            return Database.GenerateCreateScript();
        }

        /// <summary>
        /// Creates a LINQ query for the query type based on a raw SQL query.
        /// </summary>
        /// <typeparam name="TQuery">Query type.</typeparam>
        /// <param name="sql">The raw SQL query.</param>
        /// <returns>Returns an IQueryable representing the raw SQL query.</returns>
        public virtual IQueryable<TQuery> QueryFromSql<TQuery>(string sql)
            where TQuery : class
        {
            return Query<TQuery>().FromSql(sql);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>Returns the number of state entries written to the database.</returns>
        public new virtual int SaveChanges()
        {
            _auditHandler?.Handle(this);
            return base.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public new virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_auditHandler != null)
                await _auditHandler.HandleAsync(this);

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being operated on by this set.</typeparam>
        /// <returns>Returns a set for the given entity type.</returns>
        public new virtual DbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Modify the input SQL query by adding passed parameters.
        /// </summary>
        /// <param name="sql">The raw SQL query.</param>
        /// <param name="parameters">The values to be assigned to parameters.</param>
        /// <returns>Modified raw SQL query.</returns>
        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }
    }
}
