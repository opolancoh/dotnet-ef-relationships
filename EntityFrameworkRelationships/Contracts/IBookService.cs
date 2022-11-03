using EntityFrameworkRelationships.DTOs;

namespace EntityFrameworkRelationships.Contracts;

public interface IBookService
{
    Task<IEnumerable<BookDetailDto>> GetAll();
    Task<BookDetailDto?> GetById(Guid id);
    Task<BookDto> Create(BookForCreatingUpdatingDto item);
    Task<BookDto> Update(Guid id, BookForCreatingUpdatingDto item);
    Task Remove(Guid id);
}