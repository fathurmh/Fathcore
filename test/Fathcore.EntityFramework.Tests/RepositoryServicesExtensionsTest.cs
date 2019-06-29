using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Tests.Fakes;
using Fathcore.Extensions.DependencyInjection;
using Fathcore.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.EntityFramework.Tests
{
    public class RepositoryServicesExtensionsTest
    {
        IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();

        [Fact]
        public void Should_AddGenericRepository_ToService_Default()
        {
            ServiceDescriptors.AddDbContext<TestDbContext>(p => p.UseInMemoryDatabase("Should_AddGenericRepository_ToService_Default"));
            ServiceDescriptors.AddScoped<IDbContext>(p => p.GetRequiredService<TestDbContext>());
            ServiceDescriptors.AddGenericRepository();
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();

            var result = ServiceDescriptors.Where(p => p.ServiceType == typeof(IRepository<>));
            var service = serviceProvider.GetRequiredService<IRepository<CustomEntity>>();

            Assert.NotNull(result);
            Assert.Equal(typeof(Repository<CustomEntity>), service.GetType());
        }

        [Fact]
        public void Should_AddGenericRepository_ToService_CustomImplementation()
        {
            ServiceDescriptors.AddDbContext<TestDbContext>();
            ServiceDescriptors.AddScoped<IDbContext>(p => p.GetRequiredService<TestDbContext>());
            ServiceDescriptors.AddGenericRepository(typeof(CustomRepository<>));
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();

            var result = ServiceDescriptors.Where(p => p.ServiceType == typeof(IRepository<>));
            var service = serviceProvider.GetRequiredService<IRepository<CustomEntity>>();

            Assert.NotNull(result);
            Assert.NotEqual(typeof(Repository<CustomEntity>), service.GetType());
            Assert.Equal(typeof(CustomRepository<CustomEntity>), service.GetType());
        }

        [Fact]
        public void ShouldNot_AddGenericRepository_ToService_WhenImplementationType_IsNotClass()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceDescriptors.AddGenericRepository(typeof(IRepository<>)));
        }

        [Fact]
        public void ShouldNot_AddGenericRepository_ToService_WhenImplementationType_IsDoesNotAppropriate()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceDescriptors.AddGenericRepository(typeof(RepositoryServicesExtensionsTest)));
        }
    }

    class CustomEntity : BaseEntity<CustomEntity, int> { }

    class CustomRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity<TEntity>, IBaseEntity
    {
        public IQueryable<TEntity> Table { get; }
        public IQueryable<TEntity> TableNoFilters { get; }

        public IRepository<TEntity> AsNoTracking()
        {
            throw new NotImplementedException();
        }

        public IRepository<TEntity> AsTracking()
        {
            throw new NotImplementedException();
        }

        public void Delete(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<object> keyValues)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(params object[] keyValue)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(object[] keyValue, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TEntity Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, string navigationProperty)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(params object[] keyValue)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SelectAsync(object[] keyValue, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, object>> navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(string navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(IEnumerable<string> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, string navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, string navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(string navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, string navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, Expression<Func<TEntity, object>> navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, string navigationProperty, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> SelectListAsync(Expression<Func<TEntity, bool>> predicate, IPagedInput<TEntity> pagedInput, IEnumerable<string> navigationProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
