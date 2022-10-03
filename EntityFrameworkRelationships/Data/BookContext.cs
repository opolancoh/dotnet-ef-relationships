using EntityFrameworkRelationships.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Data;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookImage> BookImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
}