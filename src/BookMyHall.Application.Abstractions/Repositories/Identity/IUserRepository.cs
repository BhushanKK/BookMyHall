using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserLoginDto?> GetForLoginAsync(string mobileNumber, CancellationToken cancellationToken = default);
    Task RecordLoginAsync(Guid userId, DateTimeOffset loginDate, CancellationToken cancellationToken = default);
    Task<PaginatedResult<User>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAddressAsync( string emailAddress, CancellationToken cancellationToken = default);
    Task RemoveUserRolesAsync(Guid userId, CancellationToken cancellationToken);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken);
}