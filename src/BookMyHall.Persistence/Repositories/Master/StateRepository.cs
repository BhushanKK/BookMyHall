using Microsoft.EntityFrameworkCore;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Persistence.Repositories;

public sealed class StateRepository(BookMyHallDbContext context) : IStateRepository
{
    public async Task AddAsync(State state, CancellationToken cancellationToken = default)
        => await context.States.AddAsync(state, cancellationToken);

    public Task UpdateAsync(State state, CancellationToken cancellationToken = default)
    {
        context.States.Update(state);
        return Task.CompletedTask;
    }

    public async Task<State?> GetByIdAsync(Guid stateId, CancellationToken cancellationToken = default)
        => await context.States
            .FirstOrDefaultAsync(x => x.StateId == stateId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<State?> GetByStateCodeAsync(string stateCode, CancellationToken cancellationToken = default)
        => await context.States.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StateCode == stateCode && !x.IsDeleted,cancellationToken);

    public async Task<State?> GetByStateNameAsync(string stateName, CancellationToken cancellationToken = default)
        => await context.States.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StateName == stateName &&!x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<State>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
       var query = context.States.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.StateName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.StateName)
            .ThenBy(x=>x.StateId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<State>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}