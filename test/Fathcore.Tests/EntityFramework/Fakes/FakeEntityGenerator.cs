using System.Collections.Generic;
using System.Linq;

namespace Fathcore.EntityFramework.Fakes
{
    internal class FakeEntityGenerator
    {
        public static List<TitleEntity> TitlesOnly
            => new List<TitleEntity>()
            {
                new TitleEntity() { Subject = "First Book Title" },
                new TitleEntity() { Subject = "Second Book Title" },
                new TitleEntity() { Subject = "Third Book Title" },
                new TitleEntity() { Subject = "Fourth Book Title" },
                new TitleEntity() { Subject = "Fifth Book Title" },
            };

        public static List<BookEntity> BooksOnly
            => new List<BookEntity>()
            {
                new BookEntity() { Description = "First Book" },
                new BookEntity() { Description = "Second Book" },
                new BookEntity() { Description = "Third Book" },
                new BookEntity() { Description = "Fourth Book" },
                new BookEntity() { Description = "Fifth Book" },
            };

        public static List<AuthorEntity> AuthorsOnly
            => new List<AuthorEntity>()
            {
                new AuthorEntity() { Name = "First Author" },
                new AuthorEntity() { Name = "Second Author" },
                new AuthorEntity() { Name = "Third Author" },
                new AuthorEntity() { Name = "Fourth Author" },
                new AuthorEntity() { Name = "Fifth Author" },
            };

        public static List<TitleEntity> Titles
            => new List<TitleEntity>()
            {
                new TitleEntity() { Subject = TitlesOnly[0].Subject, Book = BooksOnly[0] },
                new TitleEntity() { Subject = TitlesOnly[1].Subject, Book = BooksOnly[1] },
                new TitleEntity() { Subject = TitlesOnly[2].Subject, Book = BooksOnly[2] },
                new TitleEntity() { Subject = TitlesOnly[3].Subject, Book = BooksOnly[3] },
                new TitleEntity() { Subject = TitlesOnly[4].Subject, Book = BooksOnly[4] }
            };

        public static List<BookEntity> Books
            => new List<BookEntity>()
            {
                new BookEntity() { Description = BooksOnly[0].Description, Title = TitlesOnly[0] },
                new BookEntity() { Description = BooksOnly[1].Description, Title = TitlesOnly[1] },
                new BookEntity() { Description = BooksOnly[2].Description, Title = TitlesOnly[2] },
                new BookEntity() { Description = BooksOnly[3].Description, Title = TitlesOnly[3] },
                new BookEntity() { Description = BooksOnly[4].Description, Title = TitlesOnly[4] }
            };

        public static List<AuthorEntity> Authors
            => new List<AuthorEntity>()
            {
                new AuthorEntity() { Name = AuthorsOnly[0].Name, Books = Books.Skip(0).Take(2).ToList() },
                new AuthorEntity() { Name = AuthorsOnly[1].Name, Books = Books.Skip(1).Take(3).ToList() },
                new AuthorEntity() { Name = AuthorsOnly[2].Name, Books = Books.Skip(3).Take(2).ToList() },
                new AuthorEntity() { Name = AuthorsOnly[3].Name, Books = Books.Skip(0).Take(3).ToList() },
                new AuthorEntity() { Name = AuthorsOnly[4].Name, Books = Books.Skip(2).Take(3).ToList() },
            };
    }
}
