using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IHallBlockRepository
{
    Task<HallBlock?> GetByIdAsync(Guid hallBlockId,CancellationToken cancellationToken = default);

    Task<PaginatedResult<HallBlock>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);

    Task AddAsync(HallBlock hallBlock,CancellationToken cancellationToken = default);

    Task UpdateAsync(HallBlock hallBlock,CancellationToken cancellationToken = default);
    Task DeleteAsync(HallBlock hallBlock,CancellationToken cancellationToken = default);
}