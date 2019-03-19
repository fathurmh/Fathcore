using System;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Audit;
using Fathcore.EntityFramework.Extensions;
using Fathcore.EntityFramework.Fakes;
using Fathcore.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.EntityFramework
{
    public class BaseDbContextTest
    {
        public IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();

        [Fact]
        public void Should_Detach_An_Entity()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Detach_An_Entity")
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
                context.Detach(entity);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Find(author.Id);
                Assert.NotEqual("Modified", result.Name);
            }
        }

        [Fact]
        public void Detach_An_Entity_Should_Throw_When_Entity_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Detach_An_Entity_Should_Throw_When_Entity_Null")
                .Options;

            using (var context = new FakeDbContext(options))
            {
                Assert.Throws<ArgumentNullException>(() => context.Detach<AuthorEntity>(null));
                Assert.Throws<ArgumentNullException>(() => context.DetachRange<AuthorEntity>(null));
            }
        }

        [Fact]
        public void Should_Exec_Query_From_Sql()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
            }

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
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.QueryFromSql<StringQueryType>($"SELECT {nameof(AuthorEntity.Name)} AS {nameof(StringQueryType.Value)} FROM [{nameof(AuthorEntity)}]").Select(p => p.Value).FirstOrDefault();
                Assert.Equal("Modified", result);
            }
        }

        [Fact]
        public void Should_Exec_Entity_From_Sql()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
            }

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
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.EntityFromSql<AuthorEntity>($"SELECT * FROM [{nameof(AuthorEntity)}]").Select(p => p).FirstOrDefault();
                Assert.Equal("Modified", result.Name);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Exec_Sql_Command(bool ensureTransaction)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
            }

            var author = FakeEntityGenerator.Authors.First();

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlCommand("UPDATE [AuthorEntity] SET [Name] = {0} WHERE Id = {1}", ensureTransaction, null, "Modified", author.Id);
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.EntityFromSql<AuthorEntity>("SELECT * FROM [AuthorEntity] WHERE Id > 0", "0").Select(p => p).FirstOrDefault();
                Assert.Equal("Modified", result.Name);
            }
        }

        [Fact]
        public void Should_Exec_Entity_From_Sql_With_Parameters()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new FakeDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
            }

            var author = FakeEntityGenerator.Authors.First();

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var param = new SqliteParameter("Id", author.Id);
                var result = context.EntityFromSql<AuthorEntity>("SELECT * FROM [AuthorEntity] WHERE [Id] = ", param).FirstOrDefault();
                Assert.Equal(author.Name, result.Name);
            }
        }

        [Theory]
        [InlineData("Anonymous", true)]
        [InlineData(null, false)]
        public void Should_SaveChanges_An_Entity(string expectedCreatedBy, bool injectAuditHandler)
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_SaveChanges_An_Entity")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            if (injectAuditHandler)
                Engine.Current.Populate(ServiceDescriptors.AddAuditHandler<AuditHandler>().AddHttpContextAccessor());
            else
                Engine.Replace(new Infrastructure.Engine());

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                context.SaveChanges();
                Assert.Equal(expectedCreatedBy, author.CreatedBy);
            }

            Engine.Current.Populate(new ServiceCollection());
        }

        [Theory]
        [InlineData("Anonymous", true)]
        [InlineData(null, false)]
        public async Task Should_SaveChangesAsync_An_Entity(string expectedCreatedBy, bool injectAuditHandler)
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_SaveChangesAsync_An_Entity")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            if (injectAuditHandler)
                Engine.Current.Populate(ServiceDescriptors.AddAuditHandler<AuditHandler>().AddHttpContextAccessor());
            else
                Engine.Replace(new Infrastructure.Engine());

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                await context.SaveChangesAsync();
                Assert.Equal(expectedCreatedBy, author.CreatedBy);
            }

            Engine.Current.Populate(new ServiceCollection());
        }
    }
}
