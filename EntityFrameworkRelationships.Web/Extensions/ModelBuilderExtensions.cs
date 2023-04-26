using EntityFrameworkRelationships.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Web.Extensions;

public static class ModelBuilderExtensions
{
    // Deprecated - Using DataHelper.Seed to populate the DB
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var authorId1 = new Guid("1cae5fb3-49b7-4722-8fb5-2bf5dcc51bd1");
        var authorId2 = new Guid("22835374-e0c7-4464-b2bc-aca852e8eda2");
        var authorId3 = new Guid("40bac79f-0386-44cb-bbbf-09a682b11074");
        var authorId4 = new Guid("bb76a1c5-92af-407f-864f-f66f101af609");

        // Authors
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = authorId1,
                Name = "William Shakespeare",
                Email = "william.shakespeare@email.com"
            },
            new Author
            {
                Id = authorId2,
                Name = "Stephen King",
                Email = "stephen.king@email.com"
            },
            new Author
            {
                Id = authorId3,
                Name = "Gabriel García Márquez",
                Email = "gabriel.garcia@email.com"
            },
            new Author
            {
                Id = authorId4,
                Name = "J.K. Rowling",
                Email = "jk.rowling@email.com"
            }
        );

        // Books
        var bookId1 = new Guid("bb76a1c5-92af-407f-864f-f66f101af609");
        var bookTitle1 = "Book 1";

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = bookId1,
                Title = bookTitle1,
                PublishedOn = (new DateTime(2023, 10, 4)).ToUniversalTime(),
            }
        );

        modelBuilder.Entity<BookImage>().HasData(
            new BookImage
            {
                BookId = bookId1,
                Url = $"https://books.com/{bookId1}",
                Alt = $"Book Title: {bookTitle1}"
            }
        );

        modelBuilder.Entity<BookAuthor>().HasData(
            new BookAuthor
            {
                BookId = bookId1,
                AuthorId = authorId1
            }
        );
    }
}