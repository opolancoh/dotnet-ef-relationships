using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkRelationships.Web.Models;

public class BookImage
{
    public string Url { get; set; }
    public string Alt { get; set; }
    
    // One-to-one relationship (Book)
    [Key, ForeignKey("Book")]
    public Guid BookId { get; set; }
    public Book Book { get; set; }
}