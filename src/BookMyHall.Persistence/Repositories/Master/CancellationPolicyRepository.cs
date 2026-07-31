using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class CancellationPolicyRepository(BookMyHallDbContext context): ICancellationPolicyRepository
{
    public async Task AddAsync(CancellationPolicy cancellationPolicy,CancellationToken cancellationToken = default)
        => await context.CancellationPolicies.AddAsync(cancellationPolicy,cancellationToken);

    public Task UpdateAsync(CancellationPolicy cancellationPolicy,CancellationToken cancellationToken = default)
    {
        context.CancellationPolicies.Update(cancellationPolicy);
        return Task.CompletedTask;
    }

    public async Task<CancellationPolicy?> GetByIdAsync(Guid cancellationPolicyId,CancellationToken cancellationToken = default)
        => await context.CancellationPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CancellationPolicyId == cancellationPolicyId,cancellationToken);

    public async Task<CancellationPolicy?> GetByPolicyNameAsync(string policyName,CancellationToken cancellationToken = default)
        => await context.CancellationPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PolicyName == policyName,cancellationToken);

    public async Task<PaginatedResult<CancellationPolicy>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        IQueryable<CancellationPolicy> query = context.CancellationPolicies
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.PolicyName, $"%{search}%") ||
                EF.Functions.ILike(x.Description, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.PolicyName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<CancellationPolicy>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}