namespace EntityFrameworkRelationships.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
    public BookImageForAddUpdateDto Image { get; set; }
}