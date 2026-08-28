using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Persistence.Repositories;

public sealed class UserRepository(BookMyHallDbContext context)
    : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await context.Users.AddAsync(user, cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }
    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Users
        .Where(x=>x.IsDeleted==false)
        .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<UserLoginDto?> GetForLoginAsync(string mobileNumber, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(x =>x.MobileNumber == mobileNumber && x.IsActive)
            .Select(x => new UserLoginDto
            {
                UserId = x.UserId,
                MobileNumber = x.MobileNumber,
                EmailAddress = x.EmailAddress,
                FullName = x.FullName,
                PasswordHash = x.PasswordHash,
                TokenVersion = x.TokenVersion,
                ProfileImageUrl = x.ProfileImageUrl,
                Roles = x.UserRoles
                .Select(ur => new JwtRole
                {
                    RoleId = ur.Role.RoleId,
                    RoleName = ur.Role.RoleName
                })
                .ToList()
            })
        .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task RecordLoginAsync(
        Guid userId,
        DateTimeOffset loginDate,
        CancellationToken cancellationToken = default)
    {
        await context.Users
            .Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.LastLoginAt,
                        loginDate)

                    .SetProperty(
                        x => x.UpdatedBy,
                        userId)

                    .SetProperty(
                        x => x.UpdatedDate,
                        loginDate),
                cancellationToken);
    }

    public async Task<PaginatedResult<User>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query =
            context.Users
            .Where(x=>x.IsDeleted==false)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.FirstName,
                    $"%{search}%")

                || (
                    x.MiddleName != null &&
                    EF.Functions.ILike(
                        x.MiddleName,
                        $"%{search}%")
                )

                || (
                    x.LastName != null &&
                    EF.Functions.ILike(
                        x.LastName,
                        $"%{search}%")
                )

                || EF.Functions.ILike(
                    x.MobileNumber,
                    $"%{search}%")

                || (
                    x.EmailAddress != null &&
                    EF.Functions.ILike(
                        x.EmailAddress,
                        $"%{search}%")
                ));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
                .OrderBy(x => x.FirstName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<User?> GetByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync
        (
            x => x.IsActive && x.EmailAddress != null && 
            EF.Functions.ILike(x.EmailAddress,emailAddress),
            cancellationToken
        );
    }

    public async Task RemoveUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
       => await context.UserRoles.Where(x => x.UserId == userId).ExecuteDeleteAsync(cancellationToken);

    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
       => await context.UserRoles.AddAsync(userRole, cancellationToken);
}