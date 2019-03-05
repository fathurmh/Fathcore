using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Audit;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Audit
{
    public class AuditHandlerTest
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IHttpContextAccessor HttpContextAccessor
        {
            get
            {
                if (_httpContextAccessor == null)
                {
                    var mock = new Mock<IHttpContextAccessor>();
                    var context = new DefaultHttpContext();
                    context.User = new GenericPrincipal(new GenericIdentity("TestIdentity"), null);
                    mock.Setup(accessor => accessor.HttpContext).Returns(context);
                    _httpContextAccessor = mock.Object;
                }

                return _httpContextAccessor;
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Add()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Add")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                context.Add(testEntity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).First(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Null(child.ModifiedBy);
                    Assert.Null(child.DeletedTime);
                });
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Modify()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Modify")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                context.Add(testEntity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<TestEntity>().Find(testEntity.Id);
                entity.TestField = "Modified";
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).First(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Null(child.ModifiedBy);
                    Assert.Null(child.DeletedTime);
                });
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Remove()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                context.Add(testEntity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).First(prop => prop.Id == testEntity.Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).IgnoreQueryFilters().First(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Equal("TestIdentity", child.ModifiedBy);
                    Assert.True(child.IsDeleted);
                    Assert.Equal(child.ModifiedTime, child.DeletedTime);
                });
            }
        }

        #region Async

        [Fact]
        public async Task Should_Asynchronously_Handle_Audit_Db_Context_When_Add()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Add")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                await context.AddAsync(testEntity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).FirstAsync(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Null(child.ModifiedBy);
                    Assert.Null(child.DeletedTime);
                });
            }
        }

        [Fact]
        public async Task Should_Asynchronously_Handle_Audit_Db_Context_When_Modify()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Modify")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                await context.AddAsync(testEntity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = await context.Set<TestEntity>().FindAsync(testEntity.Id);
                entity.TestField = "Modified";
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).FirstAsync(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Null(child.ModifiedBy);
                    Assert.Null(child.DeletedTime);
                });
            }
        }

        [Fact]
        public async Task Should_Asynchronously_Handle_Audit_Db_Context_When_Remove()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Remove")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new TestDbContext(options))
            {
                await context.AddAsync(testEntity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = await context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).FirstAsync(prop => prop.Id == testEntity.Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).IgnoreQueryFilters().FirstAsync(prop => prop.Id == testEntity.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.ChildTestEntities, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Equal("TestIdentity", child.ModifiedBy);
                    Assert.True(child.IsDeleted);
                    Assert.Equal(child.ModifiedTime, child.DeletedTime);
                });
            }
        }

        #endregion
    }
}
