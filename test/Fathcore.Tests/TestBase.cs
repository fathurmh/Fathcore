using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.Infrastructure;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        private static readonly object s_padlock = new object();
#pragma warning disable CS0618 // Type or member is obsolete
        private static readonly ILoggerFactory s_myLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) }).AddDebug().AddConsole();
#pragma warning restore CS0618 // Type or member is obsolete

        public const string DefaultIdentity = "TestIdentity";

        private IDictionary<string, DbContextOptions> _dbContextOptions = null;
        private SqliteConnection _connection = null;
        private IHttpContextAccessor _httpContextAccessor = null;
        private IServiceCollection _serviceDescriptors = null;

        private SqliteConnection Connection
        {
            get
            {
                lock (s_padlock)
                {
                    if (_connection == null)
                    {
                        _connection = new SqliteConnection($"DataSource=:memory:");
                    }

                    return _connection;
                }
            }
        }

        private IDictionary<string, DbContextOptions> DbContextOptions
        {
            get
            {
                lock (s_padlock)
                {
                    if (_dbContextOptions == null)
                    {
                        _dbContextOptions = new ConcurrentDictionary<string, DbContextOptions>();
                    }

                    return _dbContextOptions;
                }
            }
        }

        public IHttpContextAccessor HttpContextAccessor
        {
            get
            {
                lock (s_padlock)
                {
                    if (_httpContextAccessor == null)
                    {
                        var mock = new Mock<IHttpContextAccessor>();
                        var context = new DefaultHttpContext()
                        {
                            User = new GenericPrincipal(new GenericIdentity(DefaultIdentity), null)
                        };

                        mock.Setup(p => p.HttpContext).Returns(context);
                        _httpContextAccessor = mock.Object;
                    }

                    return _httpContextAccessor;
                }
            }
        }

        public IServiceCollection ServiceDescriptors
        {
            get
            {
                lock (s_padlock)
                {
                    if (_serviceDescriptors == null)
                    {
                        _serviceDescriptors = new ServiceCollection();
                    }

                    return _serviceDescriptors;
                }
            }
        }

        static TestBase() { }

        public TestBase() { }

        public DbContextOptions Options(string name, Provider provider = Provider.InMemory)
        {
            switch (provider)
            {
                case Provider.InMemory:
                {
                    if (!DbContextOptions.ContainsKey(name))
                        DbContextOptions[name] = new DbContextOptionsBuilder()
                            .UseInMemoryDatabase(databaseName: name)
                            .UseLoggerFactory(s_myLoggerFactory)
                            .EnableSensitiveDataLogging()
                            .Options;

                    using (var context = new TestDbContext(DbContextOptions[name]))
                    {
                        context.Database.EnsureCreated();
                    }

                    return DbContextOptions[name];
                }
                case Provider.Sqlite:
                {
                    Connection.Open();
                    var options = new DbContextOptionsBuilder()
                        .UseSqlite(Connection)
                        .UseLoggerFactory(s_myLoggerFactory)
                        .EnableSensitiveDataLogging()
                        .Options;

                    using (var context = new TestDbContext(options))
                    {
                        context.Database.EnsureCreated();
                        context.Database.ExecuteSqlCommand(
                        @"
                            CREATE TRIGGER SetStatusTimestamp
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

            return DbContextOptions[name];
        }

        public DbContextOptions OptionsWithData(string name, Provider provider = Provider.InMemory)
        {
            var options = Options(name, provider);
            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                context.AddRange(FakeEntityGenerator.Classrooms);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            return options;
        }

        public void Dispose()
        {
            foreach (var item in DbContextOptions)
            {
                using (var context = new TestDbContext(item.Value))
                {
                    context.Database.EnsureDeleted();
                }
            }
            Connection.Close();
            ServiceDescriptors.Clear();
            Engine.Create().Populate(ServiceDescriptors);
            BaseSingleton.AllSingletons.Clear();
        }
    }

    public enum Provider
    {
        InMemory,
        Sqlite
    }
}
