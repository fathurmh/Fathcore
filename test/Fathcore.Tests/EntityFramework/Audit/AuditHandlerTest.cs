using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fathcore.EntityFramework.AuditTrail
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
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Add")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include(prop => prop.Books).First(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.Books, child =>
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
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Modify")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<AuthorEntity>().Find(author.Id);
                entity.Name = "Modified";
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include(prop => prop.Books).First(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.Books, child =>
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
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<AuthorEntity>().Include(prop => prop.Books).First(prop => prop.Id == author.Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include(prop => prop.Books).IgnoreQueryFilters().First(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.Books, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Equal("TestIdentity", child.ModifiedBy);
                    Assert.True(child.IsDeleted);
                    Assert.Equal(child.ModifiedTime, child.DeletedTime);
                });
            }
        }

        [Fact]
        public void Should_Handle_Audit_With_Default_Principal_When_HttpContextAccessor_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_With_Default_Principal_When_HttpContextAccessor_Null")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(default);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Find(author.Id);
                Assert.Equal("Anonymous", result.CreatedBy);
            }
        }

        [Fact]
        public void Should_Handle_Audit_With_Default_Principal_When_HttpContext_Null()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_With_Default_Principal_When_HttpContext_Null")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(default(DefaultHttpContext));

            var auditHandler = new AuditHandler(mockHttpContextAccessor.Object);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Find(author.Id);
                Assert.Equal("Anonymous", result.CreatedBy);
            }
        }

        [Fact]
        public void Should_Throw_Handle_Audit_When_Context_Null()
        {
            var auditHandler = new AuditHandler(default);
            Assert.Throws<ArgumentNullException>(() => auditHandler.Handle(default));
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Remove_One_To_One_Relationship()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove_One_To_One_Relationship")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<BookEntity>().Include(prop => prop.Title).First(prop => prop.Id == author.Books.First().Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<BookEntity>().Include(prop => prop.Title).IgnoreQueryFilters().First(prop => prop.Id == author.Books.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.Equal("TestIdentity", result.Title.CreatedBy);
                Assert.Equal("TestIdentity", result.Title.ModifiedBy);
                Assert.True(result.Title.IsDeleted);
                Assert.Equal(result.Title.ModifiedTime, result.Title.DeletedTime);
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Remove_One_To_One_Relationship_2()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove_One_To_One_Relationship_2")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.Add(author);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<BookEntity>().Include(prop => prop.Title).First(prop => prop.Id == author.Books.First().Title.Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<BookEntity>().Include(prop => prop.Title).IgnoreQueryFilters().First(prop => prop.Id == author.Books.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.Equal("TestIdentity", result.Title.CreatedBy);
                Assert.Equal("TestIdentity", result.Title.ModifiedBy);
                Assert.True(result.Title.IsDeleted);
                Assert.Equal(result.Title.ModifiedTime, result.Title.DeletedTime);
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Remove_One_To_Many_Relationship()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove_One_To_Many_Relationship")
                .Options;

            var authors = FakeEntityGenerator.Authors;

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.AddRange(authors);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").First(prop => prop.Id == authors.First().Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").IgnoreQueryFilters().First(prop => prop.Id == authors.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.Books, book =>
                {
                    Assert.Equal("TestIdentity", book.CreatedBy);
                    Assert.Equal("TestIdentity", book.ModifiedBy);
                    Assert.True(book.IsDeleted);
                    Assert.Equal(book.ModifiedTime, book.DeletedTime);
                    Assert.Equal("TestIdentity", book.Title.CreatedBy);
                    Assert.Equal("TestIdentity", book.Title.ModifiedBy);
                    Assert.True(book.Title.IsDeleted);
                    Assert.Equal(book.Title.ModifiedTime, book.Title.DeletedTime);
                });
            }
        }

        [Fact]
        public void Should_Handle_Audit_Db_Context_When_Remove_One_To_Many_Relationship_2()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Db_Context_When_Remove_One_To_Many_Relationship_2")
                .Options;

            var authors = FakeEntityGenerator.Authors;

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                context.AddRange(authors);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<BookEntity>().Include(prop => prop.Title).First(prop => prop.Id == authors.First().Books.First().Title.Id);
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").IgnoreQueryFilters().First(prop => prop.Id == authors.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);
                Assert.Null(result.DeletedTime);
                Assert.Equal("TestIdentity", result.Books.First().CreatedBy);
                Assert.Equal("TestIdentity", result.Books.First().ModifiedBy);
                Assert.True(result.Books.First().IsDeleted);
                Assert.Equal(result.Books.First().ModifiedTime, result.Books.First().DeletedTime);
                Assert.Equal("TestIdentity", result.Books.First().Title.CreatedBy);
                Assert.Equal("TestIdentity", result.Books.First().Title.ModifiedBy);
                Assert.True(result.Books.First().Title.IsDeleted);
                Assert.Equal(result.Books.First().Title.ModifiedTime, result.Books.First().Title.DeletedTime);
                Assert.Equal("TestIdentity", result.Books.Last().CreatedBy);
                Assert.Null(result.Books.Last().ModifiedBy);
                Assert.False(result.Books.Last().IsDeleted);
                Assert.Null(result.Books.Last().DeletedTime);
                Assert.Equal("TestIdentity", result.Books.Last().Title.CreatedBy);
                Assert.Null(result.Books.Last().Title.ModifiedBy);
                Assert.False(result.Books.Last().Title.IsDeleted);
                Assert.Null(result.Books.Last().Title.DeletedTime);
            }
        }

        #region Async

        [Fact]
        public async Task Should_Asynchronously_Handle_Audit_Db_Context_When_Add()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Add")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include(prop => prop.Books).FirstAsync(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.Books, child =>
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
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Modify")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = await context.Set<AuthorEntity>().FindAsync(author.Id);
                entity.Name = "Modified";
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include(prop => prop.Books).FirstAsync(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.Null(result.DeletedTime);
                Assert.All(result.Books, child =>
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
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Handle_Audit_Db_Context_When_Remove")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = await context.Set<AuthorEntity>().Include(prop => prop.Books).FirstAsync(prop => prop.Id == author.Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include(prop => prop.Books).IgnoreQueryFilters().FirstAsync(prop => prop.Id == author.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.Books, child =>
                {
                    Assert.Equal("TestIdentity", child.CreatedBy);
                    Assert.Equal("TestIdentity", child.ModifiedBy);
                    Assert.True(child.IsDeleted);
                    Assert.Equal(child.ModifiedTime, child.DeletedTime);
                });
            }
        }

        [Fact]
        public async Task Should_Throw_Handle_Audit_Async_When_Context_Null()
        {
            var auditHandler = new AuditHandler(default);
            await Assert.ThrowsAsync<ArgumentNullException>(() => auditHandler.HandleAsync(default));
        }

        [Fact]
        public async Task Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_One_Relationship()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_One_Relationship")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var mock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext
            {
                User = new GenericPrincipal(new GenericIdentity("TestIdentity"), null)
            };
            mock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
            var auditHandler = new AuditHandler(mock.Object);

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = context.Set<BookEntity>().Include(prop => prop.Title).First(prop => prop.Id == author.Books.First().Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = context.Set<BookEntity>().Include(prop => prop.Title).IgnoreQueryFilters().First(prop => prop.Id == author.Books.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.Equal("TestIdentity", result.Title.CreatedBy);
                Assert.Equal("TestIdentity", result.Title.ModifiedBy);
                Assert.True(result.Title.IsDeleted);
                Assert.Equal(result.Title.ModifiedTime, result.Title.DeletedTime);
            }
        }

        [Fact]
        public async Task Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_One_Relationship_2()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_One_Relationship_2")
                .Options;

            var author = FakeEntityGenerator.Authors.First();

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddAsync(author);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = await context.Set<TitleEntity>().Include(prop => prop.Book).FirstAsync(prop => prop.Id == author.Books.First().Title.Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<TitleEntity>().Include(prop => prop.Book).IgnoreQueryFilters().FirstAsync(prop => prop.Id == author.Books.First().Title.Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.Equal("TestIdentity", result.Book.CreatedBy);
                Assert.Null(result.Book.ModifiedBy);
                Assert.False(result.Book.IsDeleted);
                Assert.Null(result.Book.DeletedTime);
            }
        }

        [Fact]
        public async Task Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_Many_Relationship()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_Many_Relationship")
                .Options;

            var authors = FakeEntityGenerator.Authors;

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddRangeAsync(authors);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = await context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").FirstAsync(prop => prop.Id == authors.First().Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").IgnoreQueryFilters().FirstAsync(prop => prop.Id == authors.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Equal("TestIdentity", result.ModifiedBy);
                Assert.True(result.IsDeleted);
                Assert.Equal(result.ModifiedTime, result.DeletedTime);
                Assert.All(result.Books, book =>
                {
                    Assert.Equal("TestIdentity", book.CreatedBy);
                    Assert.Equal("TestIdentity", book.ModifiedBy);
                    Assert.True(book.IsDeleted);
                    Assert.Equal(book.ModifiedTime, book.DeletedTime);
                    Assert.Equal("TestIdentity", book.Title.CreatedBy);
                    Assert.Equal("TestIdentity", book.Title.ModifiedBy);
                    Assert.True(book.Title.IsDeleted);
                    Assert.Equal(book.Title.ModifiedTime, book.Title.DeletedTime);
                });
            }
        }

        [Fact]
        public async Task Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_Many_Relationship_2()
        {
            var options = new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Handle_Audit_Async_Db_Context_When_Remove_One_To_Many_Relationship_2")
                .Options;

            var authors = FakeEntityGenerator.Authors;

            var auditHandler = new AuditHandler(HttpContextAccessor);

            using (var context = new FakeDbContext(options))
            {
                await context.AddRangeAsync(authors);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var entity = await context.Set<BookEntity>().Include(prop => prop.Title).FirstAsync(prop => prop.Id == authors.First().Books.First().Title.Id);
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new FakeDbContext(options))
            {
                var result = await context.Set<AuthorEntity>().Include($"{nameof(AuthorEntity.Books)}.{nameof(BookEntity.Title)}").IgnoreQueryFilters().FirstAsync(prop => prop.Id == authors.First().Id);
                Assert.Equal("TestIdentity", result.CreatedBy);
                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);
                Assert.Null(result.DeletedTime);
                Assert.Equal("TestIdentity", result.Books.First().CreatedBy);
                Assert.Equal("TestIdentity", result.Books.First().ModifiedBy);
                Assert.True(result.Books.First().IsDeleted);
                Assert.Equal(result.Books.First().ModifiedTime, result.Books.First().DeletedTime);
                Assert.Equal("TestIdentity", result.Books.First().Title.CreatedBy);
                Assert.Equal("TestIdentity", result.Books.First().Title.ModifiedBy);
                Assert.True(result.Books.First().Title.IsDeleted);
                Assert.Equal(result.Books.First().Title.ModifiedTime, result.Books.First().Title.DeletedTime);
                Assert.Equal("TestIdentity", result.Books.Last().CreatedBy);
                Assert.Null(result.Books.Last().ModifiedBy);
                Assert.False(result.Books.Last().IsDeleted);
                Assert.Null(result.Books.Last().DeletedTime);
                Assert.Equal("TestIdentity", result.Books.Last().Title.CreatedBy);
                Assert.Null(result.Books.Last().Title.ModifiedBy);
                Assert.False(result.Books.Last().Title.IsDeleted);
                Assert.Null(result.Books.Last().Title.DeletedTime);
            }
        }

        #endregion
    }
}
