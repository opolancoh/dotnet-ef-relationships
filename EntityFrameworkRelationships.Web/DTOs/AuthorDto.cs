using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EntityFrameworkRelationships.Web.DTOs;

public record AuthorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
};

public record AuthorForCreatingUpdatingDto : IValidatableObject
{
    [Required] public string Name { get; init; }
    [Required] public string Email { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Regex.IsMatch(Name, "^[a-zA-Z ']*$"))
        {
            yield return new ValidationResult(
                $"The {nameof(Name)} field is invalid.", new[] {nameof(Name)});
        }

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            yield return new ValidationResult(
                $"The {nameof(Email)} field is invalid.", new[] {nameof(Email)});
        }
    }
};

public record AuthorPlaneDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
};