using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkRelationships.Web.Models;

public class BookAuthor
{
    public Guid BookId { get; set; } // First part of composite PK; FK to Book
    public Guid AuthorId { get; set; } // Second part of composite PK; FK to Author
    
    public Book Book { get; set; }
    public Author Author { get; set; }
}