using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IEventCategoryRepository
{
    Task AddAsync(EventCategory eventCategory,CancellationToken cancellationToken = default);

    Task UpdateAsync(EventCategory eventCategory,CancellationToken cancellationToken = default);

    Task<EventCategory?> GetByIdAsync(Guid eventCategoryId,CancellationToken cancellationToken = default);

    Task<EventCategory?> GetByEventCategoryNameAsync(string eventCategoryName,CancellationToken cancellationToken = default);
    Task<EventCategory?> GetByNameIncludingDeletedAsync(string eventCategoryName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<EventCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}