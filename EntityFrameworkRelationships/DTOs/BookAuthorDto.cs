namespace EntityFrameworkRelationships.DTOs;

public record BookAuthorDto
(
    Guid BookId,
    Guid AuthorId
);