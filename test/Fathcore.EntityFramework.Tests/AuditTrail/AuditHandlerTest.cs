using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fathcore.EntityFramework.Tests.AuditTrail
{
    public class AuditHandlerTest
    {
        private static int s_count = 0;

        [Theory]
        [MemberData(nameof(HttpContextAccessorData))]
        public void AuditHandle_HasDefaultPrincipal(Provider provider, IHttpContextAccessor httpContextAccessor)
        {
            var auditHandler = new AuditHandler(httpContextAccessor);

            var options = TestHelper.Options($"AuditHandle_HasDefaultPrincipal_{s_count++}", provider);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                context.Add(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Single();

                Assert.Equal(AuditHandler.DefaultName, result.CreatedBy);
            }
        }

        [Fact]
        public void Handle_SafetyCheck()
        {
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);

            Assert.Throws<ArgumentNullException>(() => auditHandler.Handle(default));
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenAdded_ShouldPass(Provider provider)
        {
            var options = TestHelper.Options("Handle_DbContext_WhenAdded_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Add(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(TestHelper.DefaultIdentity, result.CreatedBy);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenModified_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Handle_DbContext_WhenModified_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
                entity.Code = "Modified";
                context.Update(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenDeleted_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Handle_DbContext_WhenDeleted_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().IgnoreQueryFilters().First();

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
                Assert.True(result.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;

            using (var context = new TestDbContext(options))
            {
                classroom = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").First();
                context.Remove(classroom);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").IgnoreQueryFilters().First(p => p.Id == classroom.Id);

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
                Assert.True(result.IsDeleted);

                Assert.All(result.Students, student =>
                {
                    Assert.Equal(TestHelper.DefaultIdentity, student.ModifiedBy);
                    Assert.True(student.IsDeleted);

                    Assert.Equal(TestHelper.DefaultIdentity, student.Address.ModifiedBy);
                    Assert.True(student.Address.IsDeleted);
                });
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_2(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_2", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;
            Student student;

            using (var context = new TestDbContext(options))
            {
                classroom = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").First();
                student = classroom.Students.First();
                context.Remove(student);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").IgnoreQueryFilters().First(p => p.Id == classroom.Id);

                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);

                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().ModifiedBy);
                Assert.True(result.Students.First().IsDeleted);
                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().Address.ModifiedBy);
                Assert.True(result.Students.First().Address.IsDeleted);

                Assert.Null(result.Students.Last().ModifiedBy);
                Assert.False(result.Students.Last().IsDeleted);
                Assert.Null(result.Students.Last().Address.ModifiedBy);
                Assert.False(result.Students.Last().Address.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_3(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Handle_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_3", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;
            Student student;
            Address address;

            using (var context = new TestDbContext(options))
            {
                classroom = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").First();
                student = classroom.Students.First();
                address = student.Address;
                context.Remove(address);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").IgnoreQueryFilters().First(p => p.Id == classroom.Id);

                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);

                Assert.Null(result.Students.First().ModifiedBy);
                Assert.False(result.Students.First().IsDeleted);
                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().Address.ModifiedBy);
                Assert.True(result.Students.First().Address.IsDeleted);

                Assert.Null(result.Students.Last().ModifiedBy);
                Assert.False(result.Students.Last().IsDeleted);
                Assert.Null(result.Students.Last().Address.ModifiedBy);
                Assert.False(result.Students.Last().Address.IsDeleted);
            }
        }

        [Fact]
        public async Task HandleAsync_SafetyCheck()
        {
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);

            await Assert.ThrowsAsync<ArgumentNullException>(() => auditHandler.HandleAsync(default));
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenAdded_ShouldPass(Provider provider)
        {
            var options = TestHelper.Options("HandleAsync_DbContext_WhenAdded_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                await context.AddAsync(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(TestHelper.DefaultIdentity, result.CreatedBy);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenModified_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("HandleAsync_DbContext_WhenModified_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = await context.Set<Classroom>().FirstAsync();
                entity.Code = "Modified";
                context.Update(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenDeleted_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("HandleAsync_DbContext_WhenDeleted_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = await context.Set<Classroom>().FirstAsync();
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().IgnoreQueryFilters().FirstAsync();

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
                Assert.True(result.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;

            using (var context = new TestDbContext(options))
            {
                classroom = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").FirstAsync();
                context.Remove(classroom);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}")
                    .IgnoreQueryFilters().FirstAsync(p => p.Id == classroom.Id);

                Assert.Equal(TestHelper.DefaultIdentity, result.ModifiedBy);
                Assert.True(result.IsDeleted);

                Assert.All(result.Students, student =>
                {
                    Assert.Equal(TestHelper.DefaultIdentity, student.ModifiedBy);
                    Assert.True(student.IsDeleted);

                    Assert.Equal(TestHelper.DefaultIdentity, student.Address.ModifiedBy);
                    Assert.True(student.Address.IsDeleted);
                });
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_2(Provider provider)
        {
            var options = TestHelper.OptionsWithData("HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_2", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;
            Student student;

            using (var context = new TestDbContext(options))
            {
                classroom = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").FirstAsync();
                student = classroom.Students.First();
                context.Remove(student);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}")
                    .IgnoreQueryFilters().FirstAsync(p => p.Id == classroom.Id);

                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);

                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().ModifiedBy);
                Assert.True(result.Students.First().IsDeleted);
                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().Address.ModifiedBy);
                Assert.True(result.Students.First().Address.IsDeleted);

                Assert.Null(result.Students.Last().ModifiedBy);
                Assert.False(result.Students.Last().IsDeleted);
                Assert.Null(result.Students.Last().Address.ModifiedBy);
                Assert.False(result.Students.Last().Address.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_3(Provider provider)
        {
            var options = TestHelper.OptionsWithData("HandleAsync_DbContext_WhenDeleted_WithDependentEntity_ShouldPass_3", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);
            Classroom classroom;
            Student student;
            Address address;

            using (var context = new TestDbContext(options))
            {
                classroom = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}").FirstAsync();
                student = classroom.Students.First();
                address = student.Address;
                context.Remove(address);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().Include($"{nameof(Classroom.Students)}.{nameof(Student.Address)}")
                    .IgnoreQueryFilters().FirstAsync(p => p.Id == classroom.Id);

                Assert.Null(result.ModifiedBy);
                Assert.False(result.IsDeleted);

                Assert.Null(result.Students.First().ModifiedBy);
                Assert.False(result.Students.First().IsDeleted);
                Assert.Equal(TestHelper.DefaultIdentity, result.Students.First().Address.ModifiedBy);
                Assert.True(result.Students.First().Address.IsDeleted);

                Assert.Null(result.Students.Last().ModifiedBy);
                Assert.False(result.Students.Last().IsDeleted);
                Assert.Null(result.Students.Last().Address.ModifiedBy);
                Assert.False(result.Students.Last().Address.IsDeleted);
            }
        }

        public static IEnumerable<object[]> HttpContextAccessorData()
        {
            var data = new List<object[]>();
            var mock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            data.Add(new object[] { Provider.InMemory, default(IHttpContextAccessor) });
            data.Add(new object[] { Provider.Sqlite, default(IHttpContextAccessor) });

            mock.Setup(p => p.HttpContext).Returns((HttpContext)null);
            data.Add(new object[] { Provider.InMemory, mock.Object });
            data.Add(new object[] { Provider.Sqlite, mock.Object });

            mock.Setup(p => p.HttpContext).Returns(httpContext);
            mock.Setup(p => p.HttpContext.User).Returns(default(ClaimsPrincipal));
            data.Add(new object[] { Provider.InMemory, mock.Object });
            data.Add(new object[] { Provider.Sqlite, mock.Object });

            return data;
        }
    }
}
