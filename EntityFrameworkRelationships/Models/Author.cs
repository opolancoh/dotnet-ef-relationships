namespace EntityFrameworkRelationships.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    // Many-to-many link relationship (Book)
    public IList<BookAuthor> BooksLink { get; } = new List<BookAuthor>();
}