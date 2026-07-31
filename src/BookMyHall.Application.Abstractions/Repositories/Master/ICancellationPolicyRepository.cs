using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface ICancellationPolicyRepository
{
    Task AddAsync(CancellationPolicy cancellationPolicy,CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationPolicy cancellationPolicy, CancellationToken cancellationToken = default);

    Task<CancellationPolicy?> GetByIdAsync(Guid cancellationPolicyId, CancellationToken cancellationToken = default);

    Task<CancellationPolicy?> GetByPolicyNameAsync(string policyName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<CancellationPolicy>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}