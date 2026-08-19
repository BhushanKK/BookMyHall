using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(Guid menuId,CancellationToken cancellationToken=default);
    Task<PaginatedResult<Menu>> GetAllAsync(PaginationRequest paginationRequest,
    CancellationToken cancellationToken = default);
    Task AddAsync(Menu menu,CancellationToken cancellationToken = default);
    Task UpdateAsync(Menu menu,CancellationToken cancellationToken = default);
    Task DeleteAsync(Menu menu,CancellationToken cancellationToken = default);

}