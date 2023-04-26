using EntityFrameworkRelationships.Web.Models;

namespace EntityFrameworkRelationships.Web.Data;

public static class DataHelper
{
    public static void Seed(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        // Authors
        var authorIdShakespeare = new Guid("1cae5fb3-49b7-4722-8fb5-2bf5dcc51bd1");
        var authorIdStephenKing = new Guid("22835374-e0c7-4464-b2bc-aca852e8eda2");
        var authorIdGabrielGarcia = new Guid("40bac79f-0386-44cb-bbbf-09a682b11074");
        var authorIdJKRowling = new Guid("bb76a1c5-92af-407f-864f-f66f101af609");
        var authorIdDennisRitchie = new Guid("6981577c-0687-40fb-b1c7-42b5900d9b75");
        var authorIdBrianKernighan = new Guid("7a90d435-dbbd-4855-b243-498e2659d021");

        var authors = new List<Author>
        {
            new()
            {
                Id = authorIdShakespeare,
                Name = "William Shakespeare",
                Email = "william.shakespeare@email.com"
            },
            new()
            {
                Id = authorIdStephenKing,
                Name = "Stephen King",
                Email = "stephen.king@email.com"
            },
            new()
            {
                Id = authorIdGabrielGarcia,
                Name = "Gabriel García Márquez",
                Email = "gabriel.garcia@email.com"
            },
            new()
            {
                Id = authorIdJKRowling,
                Name = "J.K. Rowling",
                Email = "jk.rowling@email.com"
            },
            new()
            {
                Id = authorIdDennisRitchie,
                Name = "Dennis Ritchie",
                Email = "dennis.ritchie@email.com"
            },
            new()
            {
                Id = authorIdBrianKernighan,
                Name = "Brian Kernighan",
                Email = "brian.kernighan@email.com"
            }
        };

        // Books
        var bookIdHamlet = new Guid("72d95bfd-1dac-4bc2-adc1-f28fd43777fd");
        var bookTitleHamlet = "Hamlet";

        var bookIdDarkTower = new Guid("c32cc263-a7af-4fbd-99a0-aceb57c91f6b");
        var bookTitleDarkTower = "The Dark Tower: The Gunslinger";

        var bookIdCienAniosSoledad = new Guid("7b6bf2e3-5d91-4e75-b62f-7357079acc51");
        var bookTitleCienAniosSoledad = "Cien Años De Soledad";

        var bookIdPhilosopherStone = new Guid("01692cba-f59b-4f02-9ee2-fcbbb04a7b29");
        var bookTitlePhilosopherStone = "Harry Potter and the Philosopher's Stone";

        var bookIdProgramming = new Guid("cc44c0c4-899b-45bf-9184-8fecc058df8b");
        var bookTitleProgramming = "The C Programming Language";

        var books = new List<Book>
        {
            new()
            {
                Id = bookIdHamlet,
                Title = bookTitleHamlet,
                PublishedOn = new DateTime(1601, 01, 01).ToUniversalTime(),
                Image = new BookImage { Url = $"https://books.com/{bookIdHamlet}", Alt = $"Title: {bookTitleHamlet}" },
                AuthorsLink = new List<BookAuthor>
                {
                    new()
                    {
                        BookId = bookIdHamlet,
                        AuthorId = authorIdShakespeare
                    }
                }
            },
            new()
            {
                Id = bookIdDarkTower,
                Title = bookTitleDarkTower,
                PublishedOn = new DateTime(1982, 06, 30).ToUniversalTime(),
                Image = new BookImage
                    { Url = $"https://books.com/{bookIdDarkTower}", Alt = $"Title: {bookTitleDarkTower}" },
                AuthorsLink = new List<BookAuthor>
                {
                    new()
                    {
                        BookId = bookIdDarkTower,
                        AuthorId = authorIdStephenKing
                    }
                }
            },
            new()
            {
                Id = bookIdCienAniosSoledad,
                Title = bookTitleCienAniosSoledad,
                PublishedOn = new DateTime(1967, 05, 01).ToUniversalTime(),
                Image = new BookImage
                {
                    Url = $"https://books.com/{bookIdCienAniosSoledad}", Alt = $"Title: {bookTitleCienAniosSoledad}"
                },
                AuthorsLink = new List<BookAuthor>
                {
                    new()
                    {
                        BookId = bookIdCienAniosSoledad,
                        AuthorId = authorIdGabrielGarcia
                    }
                }
            },
            new()
            {
                Id = bookIdPhilosopherStone,
                Title = bookTitlePhilosopherStone,
                PublishedOn = new DateTime(1997, 06, 26).ToUniversalTime(),
                Image = new BookImage
                {
                    Url = $"https://books.com/{bookIdPhilosopherStone}", Alt = $"Title: {bookTitlePhilosopherStone}"
                },
                AuthorsLink = new List<BookAuthor>
                {
                    new()
                    {
                        BookId = bookIdPhilosopherStone,
                        AuthorId = authorIdJKRowling
                    }
                }
            },
            new()
            {
                Id = bookIdProgramming,
                Title = bookTitleProgramming,
                PublishedOn = new DateTime(1978, 01, 01).ToUniversalTime(),
                Image = new BookImage
                    { Url = $"https://books.com/{bookIdProgramming}", Alt = $"Title: {bookTitleProgramming}" },
                AuthorsLink = new List<BookAuthor>
                {
                    new()
                    {
                        BookId = bookIdProgramming,
                        AuthorId = authorIdDennisRitchie
                    },
                    new()
                    {
                        BookId = bookIdProgramming,
                        AuthorId = authorIdBrianKernighan
                    }
                }
            }
        };

        // Reviews
        var reviews = new List<Review>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "Was for my daughter she is still reading it she is into the old books.",
                Rating = 4,
                BookId = bookIdHamlet
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "The book arrived in bad shape.",
                Rating = 2,
                BookId = bookIdHamlet
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "A good story told in great detail.",
                Rating = 5,
                BookId = bookIdDarkTower
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "I usually rea horror but sometimes enjoy a good fantasy book once in awhile.",
                Rating = 4,
                BookId = bookIdDarkTower
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "If you're going to learn C. Get this book. Period.",
                Rating = 5,
                BookId = bookIdProgramming
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "I should start by saying that I am not a novice developer. Nor free from the bias of academic training.",
                Rating = 4,
                BookId = bookIdProgramming
            },
            new()
            {
                Id = Guid.NewGuid(),
                Comment = "Like most technical programming literature, there is always a need to supplement.",
                Rating = 3,
                BookId = bookIdProgramming
            }
        };

        context?.Database.EnsureCreated();
        context?.Authors.AddRange(authors);
        context?.Books.AddRange(books);
        context?.Reviews.AddRange(reviews);
        context?.SaveChanges();
    }
}