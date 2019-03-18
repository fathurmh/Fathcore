using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.EntityFramework.Extensions
{
    public class DbContextExtensionsTest
    {
        [Fact]
        public void Should_Execute_Sql_Script()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
                var result = context
                    .QueryFromSql<StringQueryType>("SELECT name as Value FROM sqlite_master WHERE type='table'")
                    .Select(p => p.Value).ToList();

                Assert.Contains(nameof(AuthorEntity), result);
                Assert.Contains(nameof(BookEntity), result);
                Assert.Contains(nameof(TitleEntity), result);
            }
        }

        [Fact]
        public void Should_Execute_Sql_Script_With_Go_Statements()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var script = $"GO{Environment.NewLine}" +
                    $"{context.GenerateCreateScript()}{Environment.NewLine}" +
                    $"DROP TABLE {nameof(AuthorEntity)};{Environment.NewLine}" +
                    $"DROP TABLE {nameof(BookEntity)};{Environment.NewLine}" +
                    $"DROP TABLE {nameof(TitleEntity)};{Environment.NewLine}" +
                    $"GO 5{Environment.NewLine}" +
                    $"{context.GenerateCreateScript()}";
                context.ExecuteSqlScript(script);
                var result = context
                    .QueryFromSql<StringQueryType>("SELECT name as Value FROM sqlite_master WHERE type='table'")
                    .Select(p => p.Value).ToList();

                Assert.Contains(nameof(AuthorEntity), result);
                Assert.Contains(nameof(BookEntity), result);
                Assert.Contains(nameof(TitleEntity), result);
            }
        }

        [Fact]
        public void Should_Not_Execute_Sql_Script_When_Context_Null()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                Assert.Throws<ArgumentNullException>(() => ((IDbContext)null).ExecuteSqlScript(context.GenerateCreateScript()));
            }
        }

        [Fact]
        public void Should_Execute_Sql_Script_From_File()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql");
                File.WriteAllText(filePath, context.GenerateCreateScript());
                context.ExecuteSqlScriptFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql"));

                var result = context
                    .QueryFromSql<StringQueryType>("SELECT name as Value FROM sqlite_master WHERE type='table'")
                    .Select(p => p.Value).ToList();

                Assert.Contains(nameof(AuthorEntity), result);
                Assert.Contains(nameof(BookEntity), result);
                Assert.Contains(nameof(TitleEntity), result);
            }
        }

        [Fact]
        public void Should_Not_Execute_Sql_Script_From_File_When_Context_Null()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql");
                File.WriteAllText(filePath, context.GenerateCreateScript());
                Assert.Throws<ArgumentNullException>(() => ((IDbContext)null).ExecuteSqlScriptFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql")));
            }
        }

        [Fact]
        public void Should_Not_Execute_Sql_Script_From_File_When_File_Doesnt_Exists()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts.sql");
                Assert.Throws<FileNotFoundException>(() => context.ExecuteSqlScriptFromFile(filePath));
            }
        }

        [Fact]
        public void Should_Get_Current_Entries()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Get_Current_Entries")
                .Options;

            var authors = FakeEntityGenerator.Authors;

            using (var context = new FakeDbContext(options))
            {
                context.AddRange(authors);

                var result = context.GetCurrentEntries().ToList();

                Assert.Equal(authors.Count + authors.Sum(p => p.Books.Count * 2), result.Count);
            }
        }

        [Fact]
        public void Should_Not_Get_Current_Entries_When_Context_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Get_Current_Entries")
                .Options;

            using (var context = new FakeDbContext(options))
            {
                Assert.Throws<ArgumentNullException>(() => ((IDbContext)null).GetCurrentEntries());
            }
        }

        [Fact]
        public void Should_Rollback_Entity_Changes()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Rollback_Entity_Changes")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<AuthorEntity>().Find(author.Id);
                entity.Name = "Modified";
                context.Update(entity);
                context.RollbackEntityChanges(new DbUpdateException("DbUpdateException", new Exception()));
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include(prop => prop.Books).First(prop => prop.Id == author.Id);
                Assert.NotEqual("Modified", result.Name);
            }
        }

        [Fact]
        public void Should_Not_Rollback_Entity_Changes_When_Context_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Rollback_Entity_Changes")
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var result = ((IDbContext)null).RollbackEntityChanges(new DbUpdateException("DbUpdateException", new Exception()));
                Assert.Contains(nameof(ArgumentNullException), result);
            }
        }

        #region Async

        [Fact]
        public async Task Should_Asynchronously_Rollback_Entity_Changes()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Rollback_Entity_Changes")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<AuthorEntity>().Find(author.Id);
                entity.Name = "Modified";
                context.Update(entity);
                await context.RollbackEntityChangesAsync(new DbUpdateException("DbUpdateException", new Exception()));
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include(prop => prop.Books).FirstAsync(prop => prop.Id == author.Id);
                Assert.NotEqual("Modified", result.Name);
            }
        }

        [Fact]
        public async Task Should_Not_Asynchronously_Rollback_Entity_Changes_When_Context_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Rollback_Entity_Changes")
                .Options;

            using (var context = new FakeDbContext(options))
            {
                var result = await ((IDbContext)null).RollbackEntityChangesAsync(new DbUpdateException("DbUpdateException", new Exception()));
                Assert.Contains(nameof(ArgumentNullException), result);
            }
        }

        #endregion
    }
}
