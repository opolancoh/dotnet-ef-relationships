namespace EntityFrameworkRelationships.Web.DTOs;

public record BookAuthorDto
(
    Guid BookId,
    Guid AuthorId
);