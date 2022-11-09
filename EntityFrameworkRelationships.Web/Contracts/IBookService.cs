using EntityFrameworkRelationships.Web.DTOs;

namespace EntityFrameworkRelationships.Web.Contracts;

public interface IBookService
{
    Task<IEnumerable<BookDetailDto>> GetAll();
    Task<BookDetailDto?> GetById(Guid id);
    Task<BookDto> Create(BookForCreatingDto item);
    Task<BookDto> Update(Guid id, BookForUpdatingDto item);
    Task Remove(Guid id);
}