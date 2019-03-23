using System;
using System.Collections.Generic;
using System.Security.Principal;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.Infrastructure;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        private static readonly object s_padlock = new object();
        private readonly IDictionary<string, DbContextOptions> _options;
        private SqliteConnection _connection;
        private IHttpContextAccessor _httpContextAccessor;

        public IServiceCollection ServiceDescriptors;
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

        public const string DefaultIdentity = "TestIdentity";

        public TestBase()
        {
            _options = new Dictionary<string, DbContextOptions>();
            ServiceDescriptors = new ServiceCollection();
        }

        public DbContextOptions Options(string name, Provider provider = Provider.InMemory)
        {
            switch (provider)
            {
                case Provider.InMemory:
                {
                    if (!_options.ContainsKey(name))
                        _options[name] = new DbContextOptionsBuilder()
                            .UseInMemoryDatabase(databaseName: name)
                            .EnableSensitiveDataLogging()
                            .Options;
                }
                break;
                case Provider.Sqlite:
                {
                    name = "memory";
                    _connection = new SqliteConnection($"DataSource=:{name}:");
                    _connection.Open();
                    _options[name] = new DbContextOptionsBuilder()
                        .UseSqlite(_connection)
                        .EnableSensitiveDataLogging()
                        .Options;
                    using (var context = new TestDbContext(_options[name]))
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
                }
                break;
            }

            return _options[name];
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
            foreach (var item in _options)
            {
                using (var context = new TestDbContext(item.Value))
                {
                    context.Database.EnsureDeleted();
                }
            }
            _connection?.Close();
            _options.Clear();
            ServiceDescriptors = new ServiceCollection();
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
