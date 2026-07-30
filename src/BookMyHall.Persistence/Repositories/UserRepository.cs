using Microsoft.EntityFrameworkCore;

using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
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

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<User?> GetForLoginAsync(string mobileNumber, CancellationToken cancellationToken = default)
        => await context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.MobileNumber == mobileNumber  && x.IsActive, cancellationToken);

    public async Task<PaginatedResult<User>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.FirstName, $"%{search}%") ||
                (x.MiddleName != null && EF.Functions.ILike(x.MiddleName, $"%{search}%")) ||
                (x.LastName != null && EF.Functions.ILike(x.LastName, $"%{search}%")) ||
                EF.Functions.ILike(x.MobileNumber, $"%{search}%") ||
                (x.EmailAddress != null && EF.Functions.ILike(x.EmailAddress, $"%{search}%")));
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
}