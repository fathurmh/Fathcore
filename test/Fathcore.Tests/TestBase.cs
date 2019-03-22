using System;
using System.Collections.Generic;
using Fathcore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Tests
{
    public class TestBase : IDisposable
    {
        private readonly IDictionary<string, DbContextOptions> _options;

        public IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();

        public TestBase()
        {
            _options = new Dictionary<string, DbContextOptions>();
            BaseSingleton.AllSingletons.Clear();
        }

        public DbContextOptions Options(string name)
        {
            DbContextOptions options;
            if (_options.TryGetValue(name, out var value))
                options = value;
            else
                _options[name] = options = new DbContextOptionsBuilder().UseInMemoryDatabase(databaseName: name).Options;

            return options;
        }

        public void Dispose()
        {
            BaseSingleton.AllSingletons.Clear();
        }
    }
}
