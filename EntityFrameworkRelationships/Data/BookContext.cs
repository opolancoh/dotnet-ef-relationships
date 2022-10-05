using EntityFrameworkRelationships.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Data;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookAuthor>()
            .HasKey(x => new {x.BookId, x.AuthorId});

        // If you name your foreign keys correctly, then you don't need this.
        modelBuilder.Entity<BookAuthor>()
            .HasOne(pt => pt.Book)
            .WithMany(p => p.AuthorsLink)
            .HasForeignKey(pt => pt.BookId);
        // If you name your foreign keys correctly, then you don't need this.
        modelBuilder.Entity<BookAuthor>()
            .HasOne(pt => pt.Author)
            .WithMany(t => t.BooksLink)
            .HasForeignKey(pt => pt.AuthorId);
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookImage> BookImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DbSet<BookAuthor> BookAuthors { get; set; }
}