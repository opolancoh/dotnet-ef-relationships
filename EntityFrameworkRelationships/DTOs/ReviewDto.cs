namespace EntityFrameworkRelationships.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public Guid BookId  { get; set; }
}