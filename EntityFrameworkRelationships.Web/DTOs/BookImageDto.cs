namespace EntityFrameworkRelationships.Web.DTOs;

public record BookImageDto
{
    public string Url { get; init; }
    public string Alt { get; init; }
};