using EntityFrameworkRelationships.Models;

namespace EntityFrameworkRelationships.DTOs;

public class BookAddUpdateResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
    public BookImageDto Image { get; set; }
    
    public IEnumerable<Guid> Authors { get; set; }
}