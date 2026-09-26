using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorRepository
{
    Task AddAsync(Vendors vendor,CancellationToken cancellationToken = default);
    Task UpdateAsync(Vendors vendor,CancellationToken cancellationToken = default);
    Task<Vendors?> GetByIdAsync( Guid vendorId,CancellationToken cancellationToken = default);
    Task<Vendors?> GetByBusinessNameAsync(string businessName,CancellationToken cancellationToken = default);
    Task<Vendors?> GetByBusinessNameIncludingDeletedAsync(string businessName,CancellationToken cancellationToken = default);
    Task<PaginatedResult<Vendors>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}