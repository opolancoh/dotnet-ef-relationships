using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using EntityFrameworkRelationships.Web.Data.Validations;

namespace EntityFrameworkRelationships.Web.DTOs;

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

public record BookForCreatingDto : IValidatableObject
{
    [Required] public string? Title { get; init; }
    [Required] public DateTime? PublishedOn { get; init; }
    [Required] public BookImageDto? Image { get; init; }
    [Required] public IEnumerable<Guid>? Authors { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(BookValidation.ValidateTitle(Title!, nameof(Title)));

        return validationResult;
    }
};

public record BookForUpdatingDto : BookForCreatingDto
{
    [Required] public Guid? Id { get; init; }
}