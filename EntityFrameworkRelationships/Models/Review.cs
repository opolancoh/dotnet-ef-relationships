namespace EntityFrameworkRelationships.Models;

public class Review
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    
    // One-to-many relationship (Book)
    public Guid BookId  { get; set; }
    public Book Book  { get; set; }
}