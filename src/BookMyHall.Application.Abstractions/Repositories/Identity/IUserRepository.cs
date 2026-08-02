using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetForLoginAsync(string mobileNumber, CancellationToken cancellationToken = default);
    Task<PaginatedResult<User>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAddressAsync( string emailAddress, CancellationToken cancellationToken = default);
}