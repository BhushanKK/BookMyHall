using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorRepository
{
    Task AddAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByBusinessNameAsync(string businessName, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByBusinessNameIncludingDeletedAsync(string businessName, CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorListView>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}