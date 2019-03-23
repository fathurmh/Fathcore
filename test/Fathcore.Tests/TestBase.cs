using System;
using System.Collections.Generic;
using System.Security.Principal;
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
        private readonly IDictionary<string, DbContextOptions> _options;
        private SqliteConnection _connection;

        public IServiceCollection ServiceDescriptors;
        public IHttpContextAccessor HttpContextAccessor;

        public TestBase()
        {
            _options = new Dictionary<string, DbContextOptions>();
            ServiceDescriptors = new ServiceCollection();

            var mock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext()
            {
                User = new GenericPrincipal(new GenericIdentity("TestIdentity"), null)
            };

            mock.Setup(p => p.HttpContext).Returns(context);
            HttpContextAccessor = mock.Object;
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
                    using (DbContext context = new TestDbContext(_options[name]))
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

            using (DbContext context = new TestDbContext(options))
            {
                context.AddRange(FakeEntityGenerator.Classrooms);
                context.SaveChanges();
            }

            return options;
        }

        public void Dispose()
        {
            foreach (var item in _options)
            {
                using (DbContext context = new TestDbContext(item.Value))
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
