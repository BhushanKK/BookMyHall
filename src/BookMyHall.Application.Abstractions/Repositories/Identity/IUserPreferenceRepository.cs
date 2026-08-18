using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IUserPreferenceRepository
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId,CancellationToken cancellationToken);
    Task AddAsync(UserPreference userPreference,CancellationToken cancellationToken);
    Task UpdateAsync(UserPreference userPreference,CancellationToken cancellationToken);
    Task<UserPreference?> GetByIdAsync(Guid userPreferenceId,CancellationToken cancellationToken = default);
    Task<PaginatedResult<UserPreference>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken= default);
}
