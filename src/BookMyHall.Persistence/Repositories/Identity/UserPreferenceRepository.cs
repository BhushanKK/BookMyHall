using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class UserPreferenceRepository(BookMyHallDbContext context): IUserPreferenceRepository
{
    public async Task<UserPreference?> GetByUserIdAsync(Guid userId,CancellationToken cancellationToken)
        => await context.UserPreferences.FirstOrDefaultAsync(x => x.UserId == userId,cancellationToken);

    public async Task AddAsync(UserPreference userPreference,CancellationToken cancellationToken)
        => await context.UserPreferences.AddAsync(userPreference,cancellationToken);

    public Task UpdateAsync(UserPreference userPreference,CancellationToken cancellationToken)
    {
        context.UserPreferences.Update(userPreference);
        return Task.CompletedTask;
    }
}