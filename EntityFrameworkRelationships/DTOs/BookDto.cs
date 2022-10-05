using EntityFrameworkRelationships.Models;

namespace EntityFrameworkRelationships.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
    public BookImageDto Image { get; set; }

    public IEnumerable<AuthorLiteDto> Authors { get; set; }
}