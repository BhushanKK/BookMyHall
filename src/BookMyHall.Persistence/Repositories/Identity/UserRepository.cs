using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

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

    // public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    //     => await context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.UserId == userId, cancellationToken);
public async Task<User?> GetByIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
{
    return await context.Users
        .Include(
            x => x.UserRoles
        )
        .ThenInclude(
            x => x.Role
        )
        .FirstOrDefaultAsync(
            x =>
                !x.IsDeleted &&
                x.UserId == userId,
            cancellationToken
        );
}
    public async Task<UserLoginDto?> GetForLoginAsync(string mobileNumber, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(x =>
                x.MobileNumber == mobileNumber.Trim() &&
                x.IsActive &&
                !x.IsDeleted)
            .Select(x => new UserLoginDto
            {
                UserId = x.UserId,
                MobileNumber = x.MobileNumber,
                EmailAddress = x.EmailAddress,
                FullName = x.FullName,
                PasswordHash = x.PasswordHash!,
                TokenVersion = x.TokenVersion,
                ProfileImageUrl = x.ProfileImageUrl,
                IsEmailVerified = x.IsEmailVerified,

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

    public async Task<PaginatedResult<UserDto>> GetAllAsync(
    PaginationRequest request,
    CancellationToken cancellationToken = default)
    {
        var query = context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchPattern = $"%{request.SearchText.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.FirstName, searchPattern) ||
                (x.MiddleName != null &&
                EF.Functions.ILike(x.MiddleName, searchPattern)) ||
                (x.LastName != null &&
                EF.Functions.ILike(x.LastName, searchPattern)) ||
                (x.MobileNumber != null &&
                EF.Functions.ILike(x.MobileNumber, searchPattern)) ||
                (x.EmailAddress != null &&
                EF.Functions.ILike(x.EmailAddress, searchPattern)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ThenBy(x => x.UserId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserDto
            {
                UserId = x.UserId,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                MobileNumber = x.MobileNumber!,
                ProfileImageUrl = x.ProfileImageUrl,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                EmailAddress = x.EmailAddress,
                IsActive = x.IsActive,
                Roles = x.UserRoles
        .Select(userRole => new Role
        {
            RoleId = userRole.RoleId,

            RoleName =userRole.Role.RoleName
        })
        .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<UserDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<User?> GetByEmailAddressAsync(
        string emailAddress,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        var normalizedEmail = emailAddress.Trim().ToLowerInvariant();

        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.IsActive &&  !x.IsDeleted && 
                    x.EmailAddress == normalizedEmail,
                cancellationToken);
    }

    public async Task RemoveUserRolesAsync(Guid userId,CancellationToken cancellationToken = default)
{
    var userRoles = await context.UserRoles
        .Where(x => x.UserId == userId)
        .ToListAsync(cancellationToken);
    context.UserRoles.RemoveRange(userRoles);
}

    public async Task AddUserRoleAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
        => await context.UserRoles.AddAsync(
            userRole,
            cancellationToken);

    public async Task<UserLoginDto?> GetForGoogleLoginAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = emailAddress.Trim().ToLowerInvariant();

        return await context.Users
            .AsNoTracking()
            .Where(x =>
                x.EmailAddress == normalizedEmail &&
                x.IsActive &&
                !x.IsDeleted)
            .Select(x => new UserLoginDto
            {
                UserId = x.UserId,
                MobileNumber = x.MobileNumber,
                EmailAddress = x.EmailAddress,
                FullName = x.FullName,
                PasswordHash = x.PasswordHash!,
                TokenVersion = x.TokenVersion,
                ProfileImageUrl = x.ProfileImageUrl,
                IsEmailVerified = x.IsEmailVerified,

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
}