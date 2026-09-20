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
            .FirstOrDefaultAsync(x=>x.CancellationPolicyId == cancellationPolicyId && !x.IsDeleted && x.IsActive,cancellationToken);
               
    public async Task<CancellationPolicy?> GetByPolicyNameAsync(string policyName,CancellationToken cancellationToken = default)
        => await context.CancellationPolicies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PolicyName == policyName && !x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<CancellationPolicy>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.CancellationPolicies.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.PolicyName, pattern) || EF.Functions.ILike(x.Description, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.PolicyName)
            .ThenBy(x => x.CancellationPolicyId)
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

     public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, 
        int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.CancellationPolicies.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.PolicyName, pattern));
        }

        return await query
            .OrderBy(x => x.PolicyName)
            .ThenBy(x => x.CancellationPolicyId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.CancellationPolicyId, x.PolicyName))
            .ToListAsync(cancellationToken);
    }
}