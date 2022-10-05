namespace EntityFrameworkRelationships.DTOs;

public class BookAuthorDto
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
}