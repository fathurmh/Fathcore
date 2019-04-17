using System.Security.Principal;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;

namespace Fathcore.EntityFramework.Tests
{
    public static class TestHelper
    {
        public const string DefaultIdentity = "TestIdentity";

        public static IHttpContextAccessor HttpContextAccessor { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }

        static TestHelper()
        {
            var mock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext()
            {
                User = new GenericPrincipal(new GenericIdentity(DefaultIdentity), null)
            };
            mock.Setup(p => p.HttpContext).Returns(context);
            HttpContextAccessor = mock.Object;

#pragma warning disable CS0618 // Type or member is obsolete
            LoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) }).AddDebug().AddConsole();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public static DbContextOptions Options(string name, Provider provider = Provider.InMemory, SqliteConnection connection = null)
        {
            switch (provider)
            {
                case Provider.InMemory:
                {
                    var options = new DbContextOptionsBuilder()
                        .UseInMemoryDatabase(databaseName: name)
                        .UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .Options;

                    using (var context = new TestDbContext(options))
                    {
                        context.Database.EnsureCreated();
                    }

                    return options;
                }
                case Provider.Sqlite:
                {
                    if (connection == null)
                        connection = new SqliteConnection($"DataSource=:memory:");

                    connection.Open();
                    var options = new DbContextOptionsBuilder()
                        .UseSqlite(connection)
                        .UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .Options;

                    using (var context = new TestDbContext(options))
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        context.Database.ExecuteSqlCommand(
                        @"
                            CREATE TRIGGER IF NOT EXISTS SetRowVersion
                            AFTER UPDATE ON Classroom
                            BEGIN
                                UPDATE Classroom
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ");
                    }

                    return options;
                }
            }

            return null;
        }

        public static DbContextOptions OptionsWithData(string name, Provider provider = Provider.InMemory)
        {
            var options = Options(name, provider);

            using (var context = new TestDbContext(options))
            {
                context.AddRange(FakeEntityGenerator.Classrooms);
                context.SaveChanges();
            }

            return options;
        }
    }

    public enum Provider
    {
        InMemory,
        Sqlite
    }
}
