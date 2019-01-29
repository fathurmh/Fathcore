using System;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.Data;
using Fathcore.Data.Abstractions;
using Fathcore.Tests.Fakes;
using Xunit;

namespace Fathcore.Tests.Data
{
    public class RepositoryTests : TestsBase
    {
        [Fact]
        public void Should_Select_All_Entities()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
            }
        }
        
        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_Property()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(prop => prop.FakeChildEntities);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count > 0));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_With_Lambda_Navigation_Property_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(prop => prop.FakeChildEntities);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count > 0));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_Property()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(nameof(FakeEntity.FakeChildEntities));

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count > 0));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_String_Lambda_Navigation_Property_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(nameof(FakeEntity.FakeChildEntities));

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count > 0));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public void Should_Select_All_Entities_With_Predicate()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(prop => prop.EntityCode == entityToSearch.EntityCode);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_With_Predicate_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(prop => prop.EntityCode == entityToSearch.EntityCode);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
            }
        }
        
        [Fact]
        public void Should_Select_All_Entities_With_Predicate_And_Lambda_Navigation_Property()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(prop => prop.EntityCode == entityToSearch.EntityCode, nav => nav.FakeChildEntities);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count == entityToSearch.FakeChildEntities.Count));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_With_Predicate_And_Lambda_Navigation_Property_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(prop => prop.EntityCode == entityToSearch.EntityCode, nav => nav.FakeChildEntities);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count == entityToSearch.FakeChildEntities.Count));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public void Should_Select_All_Entities_With_Predicate_And_String_Navigation_Property()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(prop => prop.EntityCode == entityToSearch.EntityCode, nameof(FakeEntity.FakeChildEntities));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count == entityToSearch.FakeChildEntities.Count));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public async Task Should_Select_All_Entities_With_Predicate_And_String_Navigation_Property_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(prop => prop.EntityCode == entityToSearch.EntityCode, nameof(FakeEntity.FakeChildEntities));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.EntityCode == entityToSearch.EntityCode));
                Assert.All(result, prop => Assert.True(prop.EntityName == entityToSearch.EntityName));
                Assert.All(result, prop => Assert.True(prop.FakeChildEntities.Count == entityToSearch.FakeChildEntities.Count));
                Assert.All(result, prop => Assert.All(prop.FakeChildEntities, childProp => Assert.True(childProp.Id > 0)));
            }
        }
        
        [Fact]
        public void Should_Select_Entity_With_Primary_Key()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                entityToSearch = repository.Select(prop => prop.EntityCode == entityToSearch.EntityCode).Single();
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.Select(entityToSearch.Id);

                Assert.NotNull(result);
                Assert.True(result.Id > 0);
                Assert.Contains(fakeEntities, prop => prop.EntityCode == result.EntityCode);
                Assert.Contains(fakeEntities, prop => prop.EntityName == result.EntityName);
            }
        }
        
        [Fact]
        public async Task Should_Select_Entity_With_Primary_Key_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            
            var entityToSearch = fakeEntities[2];
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                entityToSearch = repository.Select(prop => prop.EntityCode == entityToSearch.EntityCode).Single();
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = await repository.SelectAsync(entityToSearch.Id);

                Assert.NotNull(result);
                Assert.True(result.Id > 0);
                Assert.Contains(fakeEntities, prop => prop.EntityCode == result.EntityCode);
                Assert.Contains(fakeEntities, prop => prop.EntityName == result.EntityName);
            }
        }
        
        [Fact]
        public void Should_Insert_Entity()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntity = new FakeEntities()[0];
            fakeEntity.FakeChildEntities = null;

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Insert(fakeEntity);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Single(result);
                Assert.True(result.First().Id > 0);
                Assert.True(result.First().EntityCode == fakeEntity.EntityCode);
                Assert.True(result.First().EntityName == fakeEntity.EntityName);
                Assert.Equal(Principal.Identity.Name, result.First().CreatedBy);
                Assert.True(result.First().CreatedTime > DateTime.MinValue);
            }
        }
        
        [Fact]
        public async Task Should_Insert_Entity_Async()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntity = new FakeEntities()[0];
            fakeEntity.FakeChildEntities = null;

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.InsertAsync(fakeEntity);
                var saveCount = await repository.SaveChangesAsync();

                Assert.True(result.Id > 0);
                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Single(result);
                Assert.True(result.First().Id > 0);
                Assert.True(result.First().EntityCode == fakeEntity.EntityCode);
                Assert.True(result.First().EntityName == fakeEntity.EntityName);
                Assert.Equal(Principal.Identity.Name, result.First().CreatedBy);
                Assert.True(result.First().CreatedTime > DateTime.MinValue);
            }
        }
        
        [Fact]
        public void Should_Insert_Entities()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntities = new FakeEntities();
            fakeEntities.ForEach(entity => entity.FakeChildEntities = null);

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Insert(fakeEntities);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Count() > 0);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(fakeEntities.Count, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var entryDateTime = result.First().CreatedTime;

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.CreatedBy));
                Assert.All(result, prop => Assert.Equal(entryDateTime, prop.CreatedTime, TimeSpan.FromMinutes(1)));
            }
        }
        
        [Fact]
        public async Task Should_Insert_Entities_Async()
        {
            var options = DatabaseOptions<FakeDbContext>();
            var fakeEntities = new FakeEntities();
            fakeEntities.ForEach(entity => entity.FakeChildEntities = null);

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.InsertAsync(fakeEntities);
                var saveCount = await repository.SaveChangesAsync();

                Assert.True(result.Count() > 0);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(fakeEntities.Count, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();
                var entryDateTime = result.First().CreatedTime;

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.CreatedBy));
                Assert.All(result, prop => Assert.Equal(entryDateTime, prop.CreatedTime, TimeSpan.FromMinutes(1)));
            }
        }
        
        [Fact]
        public void Should_Update_Entity()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToUpdate = repository.Select(prop => prop.EntityCode == fakeEntity.EntityCode).Single();
                entityToUpdate.EntityName = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.True(result.Id == entityToUpdate.Id);
                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.Equal("Modified", singleResult.EntityName);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
            }
        }
        
        [Fact]
        public async Task Should_Update_Entity_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToUpdate = (await repository.SelectAsync(prop => prop.EntityCode == fakeEntity.EntityCode)).Single();
                entityToUpdate.EntityName = "Modified";

                var result = await repository.UpdateAsync(entityToUpdate);
                var saveCount = await repository.SaveChangesAsync();

                Assert.True(result.Id > 0);
                Assert.True(result.Id == entityToUpdate.Id);
                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.Equal("Modified", singleResult.EntityName);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
            }
        }
        
        [Fact]
        public void Should_Update_Entities()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToUpdate = repository.Select().ToList();
                entitiesToUpdate.ForEach(entity => entity.EntityName = "Modified");

                var result = repository.Update(entitiesToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.All(entitiesToUpdate, prop => Assert.True(prop.Id > 0));
                Assert.Equal(entitiesToUpdate.Count, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.Equal("Modified", prop.EntityName));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.ModifiedBy));
                Assert.All(result, prop => Assert.True(prop.ModifiedTime > prop.CreatedTime));
            }
        }
        
        [Fact]
        public async Task Should_Update_Entities_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToUpdate = (await repository.SelectAsync()).ToList();
                entitiesToUpdate.ForEach(entity => entity.EntityName = "Modified");

                var result = await repository.UpdateAsync(entitiesToUpdate);
                var saveCount = await repository.SaveChangesAsync();

                Assert.All(entitiesToUpdate, prop => Assert.True(prop.Id > 0));
                Assert.Equal(entitiesToUpdate.Count, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.Equal("Modified", prop.EntityName));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.ModifiedBy));
                Assert.All(result, prop => Assert.True(prop.ModifiedTime > prop.CreatedTime));
            }
        }
        
        [Fact]
        public void Should_Delete_Entity_With_Primary_Key()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToDelete = repository.Select(prop => prop.EntityCode == fakeEntity.EntityCode).Single();

                repository.Delete(entityToDelete.Id);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.True(singleResult.IsDeleted);
                Assert.True(singleResult.DeletedTime == singleResult.ModifiedTime);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
            }
        }
        
        [Fact]
        public async Task Should_Delete_Entity_With_Primary_Key_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToDelete = (await repository.SelectAsync(prop => prop.EntityCode == fakeEntity.EntityCode)).Single();

                await repository.DeleteAsync(entityToDelete.Id);
                var saveCount = await repository.SaveChangesAsync();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.True(singleResult.IsDeleted);
                Assert.True(singleResult.DeletedTime == singleResult.ModifiedTime);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
            }
        }
        
        [Fact]
        public void Should_Delete_Entity_With_Entity_Object()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToDelete = repository.Select(prop => prop.EntityCode == fakeEntity.EntityCode).Single();

                repository.Delete(entityToDelete);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.True(singleResult.IsDeleted);
                Assert.True(singleResult.DeletedTime == singleResult.ModifiedTime);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
            }
        }
        
        [Fact]
        public async Task Should_Delete_Entity_With_Entity_Object_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToDelete = (await repository.SelectAsync(prop => prop.EntityCode == fakeEntity.EntityCode)).Single();

                await repository.DeleteAsync(entityToDelete);
                var saveCount = await repository.SaveChangesAsync();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.True(singleResult.IsDeleted);
                Assert.True(singleResult.DeletedTime == singleResult.ModifiedTime);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
            }
        }
        
        [Fact]
        public void Should_Delete_Entities()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToDelete = repository.Select();

                repository.Delete(entitiesToDelete);
                var saveCount = repository.SaveChanges();

                Assert.Equal(entitiesToDelete.Count(), saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.IsDeleted));
                Assert.All(result, prop => Assert.True(prop.DeletedTime == prop.ModifiedTime));
                Assert.All(result, prop => Assert.True(prop.ModifiedTime > prop.CreatedTime));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.ModifiedBy));
            }
        }
        
        [Fact]
        public async Task Should_Delete_Entities_Async()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToDelete = await repository.SelectAsync();

                await repository.DeleteAsync(entitiesToDelete);
                var saveCount = await repository.SaveChangesAsync();

                Assert.Equal(entitiesToDelete.Count(), saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = await repository.SelectAsync();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.IsDeleted));
                Assert.All(result, prop => Assert.True(prop.DeletedTime == prop.ModifiedTime));
                Assert.All(result, prop => Assert.True(prop.ModifiedTime > prop.CreatedTime));
                Assert.All(result, prop => Assert.Equal(Principal.Identity.Name, prop.ModifiedBy));
            }
        }

        [Fact]
        public void Query_Tracking_Behavior_Default_To_Track_All()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToUpdate = repository.Select(prop => prop.EntityCode == fakeEntity.EntityCode).Single();
                entityToUpdate.EntityName = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.Equal("Modified", singleResult.EntityName);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
            }
        }

        [Fact]
        public void Query_Tracking_Behavior_As_Tracking_Using_Table_Queryable()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToUpdate = repository.Table.Single(prop => prop.EntityCode == fakeEntity.EntityCode);
                entityToUpdate.EntityName = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.Equal("Modified", singleResult.EntityName);
                Assert.Equal(Principal.Identity.Name, singleResult.ModifiedBy);
                Assert.True(singleResult.ModifiedTime > singleResult.CreatedTime);
            }
        }

        [Fact]
        public void Using_TableNoTracking_Queryable_Cannot_Save_Changes()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entityToUpdate = repository.TableNoTracking.Single(prop => prop.EntityCode == fakeEntity.EntityCode);
                entityToUpdate.EntityName = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var result = repository.Select();
                var singleResult = result.Single(prop => prop.EntityCode == fakeEntity.EntityCode);

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.NotEqual("Modified", singleResult.EntityName);
                Assert.NotEqual(Principal.Identity.Name, singleResult.ModifiedBy);
                Assert.False(singleResult.ModifiedTime > singleResult.CreatedTime);
            }
        }

        [Fact]
        public void Should_Select_All_Entities_Using_TableNoTracking_Queryable()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);

                var result = repository.TableNoTracking.ToList();

                Assert.Equal(fakeEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
            }
        }

        [Fact]
        public void Select_Without_Predicate_Will_Not_Tracked()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToUpdate = repository.Select().ToList();
                entitiesToUpdate.ForEach(entity => entity.EntityName = "Modified");

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Fact]
        public void Update_Entities_Using_Table_Queryable()
        {
            var options = DatabaseOptionsWithData<FakeDbContext>(typeof(FakeDbContext));
            var fakeEntities = new FakeEntities();
            var fakeEntity = fakeEntities[0];

            using (IDbContext context = new FakeDbContext(options))
            {
                var repository = new Repository<FakeEntity>(context);
                
                var entitiesToUpdate = repository.Table.ToList();
                entitiesToUpdate.ForEach(entity => entity.EntityName = "Modified");

                var saveCount = repository.SaveChanges();

                Assert.Equal(entitiesToUpdate.Count, saveCount);
            }
        }
    }
}
