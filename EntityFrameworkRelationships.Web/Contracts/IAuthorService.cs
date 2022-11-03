using EntityFrameworkRelationships.Web.DTOs;

namespace EntityFrameworkRelationships.Web.Contracts;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAll();
    Task<AuthorDto?> GetById(Guid id);
    Task<AuthorDto> Create(AuthorForCreatingUpdatingDto item);
    Task<AuthorDto> Update(Guid id, AuthorForCreatingUpdatingDto item);
    Task Remove(Guid id);
}