using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Db context extensions.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Execute commands from the SQL script against the context database
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
        /// <param name="sql">SQL script</param>
        public static void ExecuteSqlScript(this IDbContext context, string sql)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sqlCommands = GetCommandsFromScript(sql);
            foreach (var command in sqlCommands)
                context.ExecuteSqlCommand(command);
        }

        /// <summary>
        /// Execute commands from a file with SQL script against the context database.
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
        /// <param name="filePath">Path to the file.</param>
        public static void ExecuteSqlScriptFromFile(this IDbContext context, string filePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!File.Exists(filePath))
                return;

            context.ExecuteSqlScript(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Gets an <see cref="EntityEntry"/> for each entity being tracked by the context with state <see cref="EntityState.Added"/>,
        /// <see cref="EntityState.Modified"/>, <see cref="EntityState.Deleted"/> or <see cref="EntityState.Detached"/>.
        /// </summary>
        /// <param name="context">A <see cref="IDbContext"/> context.</param>
        /// <returns>Collection of EntityEntry.</returns>
        public static IEnumerable<EntityEntry> GetCurrentEntries(this IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added
                    || entry.State == EntityState.Modified
                    || entry.State == EntityState.Deleted
                    || entry.State == EntityState.Detached)
                .ToList();
        }


        /// <summary>
        /// Rollback of entity changes and return full error message.
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
        /// <param name="exception">Exception occurs.</param>
        /// <returns></returns>
        public static string RollbackEntityChanges(this IDbContext context, DbUpdateException exception)
        {
            try
            {
                context.RollbackEntity();
                context.SaveChanges();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Asynchronously rollback of entity changes and return full error message.
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
        /// <param name="exception">Exception occurs.</param>
        /// <returns></returns>
        public static async Task<string> RollbackEntityChangesAsync(this IDbContext context, DbUpdateException exception)
        {
            try
            {
                context.RollbackEntity();
                await context.SaveChangesAsync();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Rollback of entity changes.
        /// </summary>
        /// <param name="context">An <see cref="IDbContext"/> context.</param>
        private static void RollbackEntity(this IDbContext context)
        {
            var entries = ((BaseDbContext)context).GetCurrentEntries()
                .Where(entry => entry.State == EntityState.Modified
                    || entry.State == EntityState.Deleted
                    || entry.State == EntityState.Detached)
                .ToList();

            entries.ForEach(entry =>
            {
                try
                {
                    entry.State = EntityState.Unchanged;
                }
                catch (InvalidOperationException)
                {
                    // ignored.
                }
            });
        }

        /// <summary>
        /// Get SQL commands from the script.
        /// </summary>
        /// <param name="sql">SQL script.</param>
        /// <returns>List of commands.</returns>
        private static IList<string> GetCommandsFromScript(string sql)
        {
            var commands = new List<string>();

            //origin from the Microsoft.EntityFrameworkCore.Migrations.SqlServerMigrationsSqlGenerator.Generate method
            sql = Regex.Replace(sql, @"\\\r?\n", string.Empty);
            var batches = Regex.Split(sql, @"^\s*(GO[ \t]+[0-9]+|GO)(?:\s+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            for (var i = 0; i < batches.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(batches[i]) || batches[i].StartsWith("GO", StringComparison.OrdinalIgnoreCase))
                    continue;

                var count = 1;
                if (i != batches.Length - 1 && batches[i + 1].StartsWith("GO", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(batches[i + 1], "([0-9]+)");
                    if (match.Success)
                        count = int.Parse(match.Value);
                }

                var builder = new StringBuilder();
                for (var j = 0; j < count; j++)
                {
                    builder.Append(batches[i]);
                    if (i == batches.Length - 1)
                        builder.AppendLine();
                }

                commands.Add(builder.ToString());
            }

            return commands;
        }
    }
}
