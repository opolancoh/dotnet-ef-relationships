namespace EntityFrameworkRelationships.DTOs;

public record AuthorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
};

public record AuthorPlaneDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
};