using System.Collections;

namespace EntityFrameworkRelationships.DTOs;

public record BookBaseDto
{
    public string Title { get; init; }
    public DateTime PublishedOn { get; init; }
    public BookImageDto Image { get; init; }
};

public record BookDto : BookBaseDto
{
    public Guid Id { get; init; }
    public IEnumerable<Guid> Authors { get; init; }
};

public record BookDetailDto : BookBaseDto
{
    public Guid Id { get; init; }
    public IEnumerable<AuthorPlaneDto> Authors { get; init; }
}

public record BookForCreatingUpdatingDto : BookBaseDto
{
    public IEnumerable<Guid> Authors { get; init; }
};

