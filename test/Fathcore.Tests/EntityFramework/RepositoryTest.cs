using System.Collections.Generic;
using System.Linq;
using Fathcore.EntityFramework.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.EntityFramework
{
    public class RepositoryTest
    {
        #region Utilities

        private static readonly Dictionary<string, DbContextOptions<FakeDbContext>> s_optionsDict = new Dictionary<string, DbContextOptions<FakeDbContext>>();
        private static readonly List<AuthorEntity> s_authors = FakeEntityGenerator.Authors;
        private static readonly List<AuthorEntity> s_authorsOnly = FakeEntityGenerator.AuthorsOnly;

        private DbContextOptions<FakeDbContext> Options(string name) => new DbContextOptionsBuilder<FakeDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

        private DbContextOptions<FakeDbContext> OptionsWithData(string name)
        {
            if (s_optionsDict.TryGetValue(name, out var value))
                return value;

            var options = Options(name);
            s_optionsDict.Add(name, options);

            using (var context = new FakeDbContext(options))
            {
                context.AddRange(s_authors);
                context.SaveChanges();
            }

            return options;
        }

        #endregion

        #region Synchronous

        #region Select All
        [Fact]
        public void Should_Select_All_Entities_Default_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList();

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList();

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_AsNoTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList();

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_Default_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList(prop => prop.Books);

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.Books);

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_AsNoTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.Books);

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == prop.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_Default_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList(nameof(AuthorEntity.Books));

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList(nameof(AuthorEntity.Books));

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_AsNoTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList(nameof(AuthorEntity.Books));

                Assert.Equal(s_authors.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_authors.Single(p => p.Name == prop.Name).Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == prop.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList(prop => prop.Name == entityToSelect.Name);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.Name == entityToSelect.Name);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.Name == entityToSelect.Name);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.SelectList(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.Books.Count, prop.Books.Count));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == child.Id)));
            }
        }

        #endregion

        #region Select

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(prop => prop.Name == entityToSelect.Name);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.Name == entityToSelect.Name);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.Name == entityToSelect.Name);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.Name == entityToSelect.Name, prop => prop.Books);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.Contains(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.Name == entityToSelect.Name, nameof(AuthorEntity.Books));

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.Books.Count, result.Books.Count);
                Assert.All(result.Books, child => Assert.True(child.Id > 0));
                Assert.All(result.Books, child => Assert.DoesNotContain(context.Set<BookEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToSelect = repository.Select(prop => prop.Name == entityToSelect.Name);
            }

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_AsTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToSelect = repository.AsTracking().Select(prop => prop.Name == entityToSelect.Name);
            }

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_AsNoTracking(int index)
        {
            AuthorEntity entityToSelect = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToSelect = repository.AsNoTracking().Select(prop => prop.Name == entityToSelect.Name);
            }

            using (var context = new FakeDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<AuthorEntity>().Local, e => e.Id == result.Id);
            }
        }

        #endregion

        #region Insert

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Insert_Entity(int index)
        {
            AuthorEntity entityToInsert = s_authorsOnly[index];

            using (var context = new FakeDbContext(Options("Should_Insert_Entity")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Insert(entityToInsert);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Insert_Entity_With_Navigation(int index)
        {
            AuthorEntity entityToInsert = s_authors[index];

            using (var context = new FakeDbContext(Options("Should_Insert_Entity_With_Navigation")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Insert(entityToInsert);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.Equal(1 + (entityToInsert.Books.Count * 2), saveCount);
            }
        }

        [Fact]
        public void Should_Insert_Entities()
        {
            using (var context = new FakeDbContext(Options("Should_Insert_Entities")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Insert(s_authorsOnly);
                var saveCount = repository.SaveChanges();

                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(s_authorsOnly.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Insert_Entities_With_Navigation()
        {
            using (var context = new FakeDbContext(Options("Should_Insert_Entities_With_Navigation")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Insert(s_authors);
                var saveCount = repository.SaveChanges();

                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(s_authors.Count + s_authors.Sum(prop => prop.Books.Count * 2), saveCount);
            }
        }

        #endregion

        #region Update

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_Default_AsTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(prop => prop.Name == entityToUpdate.Name);
                result.Name = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.Name == entityToUpdate.Name);
                result.Name = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsNoTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.Name == entityToUpdate.Name);
                result.Name = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_Default_AsTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.Select(prop => prop.Name == entityToUpdate.Name);
                entityToUpdate.Name = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.AsTracking().Select(prop => prop.Name == entityToUpdate.Name);
                entityToUpdate.Name = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsNoTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.AsNoTracking().Select(prop => prop.Name == entityToUpdate.Name);
                entityToUpdate.Name = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_Default_AsTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                result.Name = "Modified";
                result.Books.First().Description = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                result.Name = "Modified";
                result.Books.First().Description = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsNoTracking(int index)
        {
            AuthorEntity entityToUpdate = s_authors[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                result.Name = "Modified";
                result.Books.First().Description = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_Default_AsTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authorsOnly[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                entityToUpdate.Name = "Modified";
                entityToUpdate.Books.First().Description = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authorsOnly[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.AsTracking().Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                entityToUpdate.Name = "Modified";
                entityToUpdate.Books.First().Description = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsNoTracking_Using_Method(int index)
        {
            AuthorEntity entityToUpdate = s_authorsOnly[index];

            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                entityToUpdate = repository.AsNoTracking().Select(prop => prop.Name == entityToUpdate.Name, prop => prop.Books);
                entityToUpdate.Name = "Modified";
                entityToUpdate.Books.First().Description = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1 + entityToUpdate.Books.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_Default_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsTracking().SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsNoTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsNoTracking().SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_Default_AsTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsTracking().SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsNoTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsNoTracking().SelectList();

                foreach (var result in results)
                    result.Name = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_Default_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count + s_authors.Count(p => p.Books.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsTracking().SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count + s_authors.Count(p => p.Books.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsNoTracking()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsNoTracking")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsNoTracking().SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_Default_AsTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count + s_authors.Count(p => p.Books.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsTracking().SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count + s_authors.Count(p => p.Books.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsNoTracking_Using_Method()
        {
            using (var context = new FakeDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<AuthorEntity>(context);

                var results = repository.AsNoTracking().SelectList(prop => prop.Books);

                foreach (var result in results)
                {
                    result.Name = "Modified";
                    if (result.Books.Any())
                        result.Books.First().Description = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_authors.Count + s_authors.Sum(p => p.Books.Count), saveCount);
            }
        }

        #endregion

        #region Delete

        #endregion

        #endregion

        #region Asynchronous
        #endregion
    }
}
