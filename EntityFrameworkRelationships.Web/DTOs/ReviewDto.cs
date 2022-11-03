namespace EntityFrameworkRelationships.Web.DTOs;

public record ReviewDto
{
    public Guid Id { get; init; }
    public string Comment { get; init; }
    public int Rating { get; init; }
    public Guid BookId { get; init; }
};