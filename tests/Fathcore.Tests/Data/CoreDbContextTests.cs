using System;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.Data.Abstractions;
using Fathcore.Extensions;
using Fathcore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.Tests.Data
{
    public class FakeDbContextTests : TestsBase
    {
        [Fact]
        public void Should_Build_Models()
        {
            using (FakeDbContext context = new FakeDbContext(DatabaseOptions<FakeDbContext>()))
            {
                var models = context.Model.GetEntityTypes();
                var result = models.FirstOrDefault(prop => prop.Name.Equals(typeof(FakeEntity).FullName));

                Assert.NotEmpty(models);
                Assert.NotNull(result);
            }
        }

        [Fact]
        public void Should_Set_Entity()
        {
            using (IDbContext context = new FakeDbContext(DatabaseOptions<FakeDbContext>()))
            {
                var dbSet = context.Set<FakeEntity>();
                
                Assert.NotNull(dbSet);
            }
        }

        [Fact]
        public void Should_Audit_When_EntityState_Added()
        {
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(DatabaseOptions<FakeDbContext>()))
            {
                var dbSet = context.Set<FakeEntity>();
                dbSet.AddRange(fakeEntities);
                context.Audit();
                
                var auditEntries = context.GetCurrentEntries();
                var entryDateTime = (DateTime)auditEntries.First().CurrentValues[nameof(FakeEntity.CreatedTime)];

                Assert.All(auditEntries, prop => Assert.Equal(Principal.Identity.Name, prop.CurrentValues[nameof(FakeEntity.CreatedBy)]));
                Assert.All(auditEntries, prop => Assert.Equal(entryDateTime, (DateTime)prop.CurrentValues[nameof(FakeEntity.CreatedTime)], TimeSpan.FromMinutes(1)));
                Assert.All(auditEntries, prop => Assert.Null(prop.CurrentValues[nameof(FakeEntity.ModifiedBy)]));
                Assert.All(auditEntries, prop => Assert.Null(prop.CurrentValues[nameof(FakeEntity.ModifiedTime)]));
            }
        }

        [Fact]
        public void Should_Audit_When_EntityState_Modified()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var dbSet = context.Set<FakeEntity>();
                dbSet.AddRange(fakeEntities);
                context.SaveChanges();
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var dbSet = context.Set<FakeEntity>();
                dbSet.AttachRange(fakeEntities);
                fakeEntities.ForEach(entity => entity.EntityName = "Modified");
                context.Audit();
                
                var auditEntries = context.GetCurrentEntries();
                var modifyDateTime = (DateTime)auditEntries.First().CurrentValues[nameof(FakeEntity.ModifiedTime)];

                Assert.All(auditEntries, prop => Assert.Equal("Modified", prop.CurrentValues[nameof(FakeEntity.EntityName)]));
                Assert.All(auditEntries, prop => Assert.Equal(Principal.Identity.Name, prop.CurrentValues[nameof(FakeEntity.ModifiedBy)]));
                Assert.All(auditEntries, prop => Assert.Equal(modifyDateTime, (DateTime)prop.CurrentValues[nameof(FakeEntity.ModifiedTime)], TimeSpan.FromMinutes(1)));
            }
        }

        [Fact]
        public void Should_Audit_When_EntityState_SoftDeleted()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var dbSet = context.Set<FakeEntity>();
                dbSet.AddRange(fakeEntities);
                context.SaveChanges();
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var dbSet = context.Set<FakeEntity>();
                dbSet.RemoveRange(fakeEntities);
                context.Audit();
                
                var auditEntries = context.GetCurrentEntries();

                Assert.All(auditEntries, prop => Assert.Equal(Principal.Identity.Name, prop.CurrentValues[nameof(FakeEntity.ModifiedBy)]));
                Assert.All(auditEntries, prop => Assert.True((bool)prop.CurrentValues[nameof(FakeEntity.IsDeleted)]));
                Assert.All(auditEntries, prop => Assert.Equal((DateTime)prop.CurrentValues[nameof(FakeEntity.ModifiedTime)], (DateTime)prop.CurrentValues[nameof(FakeEntity.DeletedTime)], TimeSpan.FromMinutes(1)));
            }
        }
        
        [Fact]
        public void Thrown_Exception_On_Generate_Create_Script_Non_Relational_Database()
        {
            using (IDbContext context = new FakeDbContext(DatabaseOptions<FakeDbContext>()))
            {
                Assert.Throws<InvalidOperationException>(() => context.GenerateCreateScript());
            }
        }
    }
}
