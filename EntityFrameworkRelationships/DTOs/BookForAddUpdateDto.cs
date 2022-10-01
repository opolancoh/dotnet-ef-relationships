namespace EntityFrameworkRelationships.DTOs;

public class BookForAddUpdateDto
{
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }
    public BookImageForAddUpdateDto Image { get; set; }
}