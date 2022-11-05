using EntityFrameworkRelationships.Web.DTOs;

namespace EntityFrameworkRelationships.Web.Contracts;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAll();
    Task<ReviewDto?> GetById(Guid id);
    Task<ReviewDto> Create(ReviewForCreatingDto item);
    Task<ReviewDto> Update(ReviewForUpdatingDto item);
    Task Remove(Guid id);
}