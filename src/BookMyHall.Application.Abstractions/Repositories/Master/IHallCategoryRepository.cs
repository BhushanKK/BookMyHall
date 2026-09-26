using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IHallCategoryRepository
{
    Task<HallCategory?> GetByIdAsync(Guid hallCategoryId,CancellationToken cancellationToken = default);
    Task<PaginatedResult<HallCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
    Task AddAsync(HallCategory hallCategory,CancellationToken cancellationToken = default);
    Task UpdateAsync(HallCategory hallCategory,CancellationToken cancellationToken = default);
    Task<HallCategory?> GetByNameIncludingDeletedAsync(string hallcategoryName,CancellationToken cancellationToken = default);
    Task<HallCategory?> GetByHallCategoryNameAsync(string hallCategoryName,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);

}