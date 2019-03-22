using System;
using System.Collections.Generic;
using Fathcore.Infrastructure;
using Fathcore.Tests.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        private readonly IDictionary<string, DbContextOptions> _options;
        private SqliteConnection _connection;

        public IServiceCollection ServiceDescriptors;

        public TestBase()
        {
            _options = new Dictionary<string, DbContextOptions>();
            ServiceDescriptors = new ServiceCollection();
            BaseSingleton.AllSingletons.Clear();
        }

        public DbContextOptions Options(string name, Provider provider = Provider.InMemory)
        {
            switch (provider)
            {
                case Provider.InMemory:
                {
                    if (!_options.ContainsKey(name))
                        _options[name] = new DbContextOptionsBuilder().UseInMemoryDatabase(databaseName: name).Options;
                }
                break;
                case Provider.Sqlite:
                {
                    name = "memory";
                    _connection = new SqliteConnection($"DataSource=:{name}:");
                    _connection.Open();
                    _options[name] = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
                    using (DbContext context = new TestDbContext(_options[name]))
                    {
                        context.Database.ExecuteSqlCommand(context.Database.GenerateCreateScript());
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
