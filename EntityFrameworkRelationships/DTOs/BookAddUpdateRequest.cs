namespace EntityFrameworkRelationships.DTOs;

public class BookAddUpdateRequest
{
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
    public BookImageDto Image { get; set; }
    public ICollection<Guid> Authors { get; set; }
}