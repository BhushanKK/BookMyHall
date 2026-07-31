using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class StateRepository(BookMyHallDbContext context): IStateRepository
{
    public async Task AddAsync(State state,CancellationToken cancellationToken = default)
        => await context.States.AddAsync(state, cancellationToken);

    public Task UpdateAsync(State state,CancellationToken cancellationToken = default)
    {
        context.States.Update(state);
        return Task.CompletedTask;
    }

    public async Task<State?> GetByIdAsync(Guid stateId,CancellationToken cancellationToken = default)
        => await context.States
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.StateId == stateId,
                cancellationToken);

    public async Task<State?> GetByStateCodeAsync(string stateCode,CancellationToken cancellationToken = default)
        => await context.States
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.StateCode == stateCode,
                cancellationToken);

    public async Task<State?> GetByStateNameAsync(string stateName,CancellationToken cancellationToken = default)
        => await context.States
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.StateName == stateName,
                cancellationToken);
}