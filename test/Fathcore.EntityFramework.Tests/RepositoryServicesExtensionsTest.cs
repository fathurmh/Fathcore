using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public IDbContext DbContext { get; }
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

        public void Delete(object keyValue)
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

        public TEntity Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(IPaginationData<TEntity> paginationData)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(IPaginationData<TEntity> paginationData, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(IPaginationData<TEntity> paginationData, params string[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IPagedList<TEntity> PagedList(Expression<Func<TEntity, bool>> predicate, IPaginationData<TEntity> paginationData, params string[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public TEntity Select(object keyValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(params string[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate, params string[] navigationProperties)
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
    }
}
