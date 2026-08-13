using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IHallPricingRepository
{
    Task AddAsync(HallPricing hallPricing, CancellationToken cancellationToken = default);
    Task UpdateAsync(HallPricing hallPricing, CancellationToken cancellationToken = default);
    Task<HallPricing?> GetByIdAsync(Guid hallPricingId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<HallPricing>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<HallPricing?> GetByHallIdAndEventCategoryIdAsync(Guid hallId, Guid eventCategoryId, CancellationToken cancellationToken = default);
}