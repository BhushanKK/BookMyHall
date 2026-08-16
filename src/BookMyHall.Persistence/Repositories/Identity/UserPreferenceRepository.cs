using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class UserPreferenceRepository(
    BookMyHallDbContext context) : IUserPreferenceRepository
{
    public async Task<UserPreference?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
        => await context.UserPreferences.FirstOrDefaultAsync(
            x => x.UserId == userId,
            cancellationToken);

    public async Task AddAsync(
        UserPreference userPreference,
        CancellationToken cancellationToken)
        => await context.UserPreferences.AddAsync(
            userPreference,
            cancellationToken);

    public Task UpdateAsync(
        UserPreference userPreference,
        CancellationToken cancellationToken)
    {
        context.UserPreferences.Update(userPreference);
        return Task.CompletedTask;
    }

    public async Task<UserPreference?> GetByIdAsync(
        Guid userPreferenceId,
        CancellationToken cancellationToken = default)
        => await context.UserPreferences.FirstOrDefaultAsync(
            x => x.UserPreferenceId == userPreferenceId,
            cancellationToken);

    public Task DeleteAsync(
        UserPreference userPreference,
        CancellationToken cancellationToken = default)
    {
        context.UserPreferences.Remove(userPreference);
        return Task.CompletedTask;
    }

    public async Task<PaginatedResult<UserPreference>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = context.UserPreferences
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.UserPreferenceId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<UserPreference>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}