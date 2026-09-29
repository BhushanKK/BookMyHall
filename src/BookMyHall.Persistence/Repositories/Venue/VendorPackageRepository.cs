using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Venue;
using BookMyHall.Contracts.Common;
using Microsoft.EntityFrameworkCore;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public class VendorPackageRepository(BookMyHallDbContext context) : IVendorPackageRepository
{
    public async Task<VendorPackage?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.VendorPackages
            .FirstOrDefaultAsync(x => x.VendorPackageId == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<VendorPackage?> GetByNameIncludingDeletedAsync(string packageName, Guid vendorId, CancellationToken cancellationToken)
    {
        return await context.VendorPackages
            .FirstOrDefaultAsync(x => x.PackageName.ToLower() == packageName.ToLower() && x.VendorId == vendorId, cancellationToken);
    }

    public async Task AddAsync(VendorPackage entity, CancellationToken cancellationToken)
    {
        await context.VendorPackages.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(VendorPackage entity, CancellationToken cancellationToken)
    {
        context.VendorPackages.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<PaginatedResult<VendorPackage>> GetAllAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var query = context.VendorPackages.AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchText))
        {
            var search = paginationRequest.SearchText.ToLower();
            query = query.Where(x => x.PackageName.ToLower().Contains(search) || 
                                     (x.Description != null && x.Description.ToLower().Contains(search)));
        }
        query = paginationRequest.SortDescending 
            ? query.OrderByDescending(x => x.DisplayOrder) 
            : query.OrderBy(x => x.DisplayOrder);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new PaginatedResult<VendorPackage>
        {
            Items = items,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize,
            TotalCount = totalCount
        };
    }

      public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.VendorPackages.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.PackageName, pattern));
        }

        return await query
            .OrderBy(x => x.PackageName)
            .ThenBy(x => x.VendorPackageId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.VendorPackageId, x.PackageName))
            .ToListAsync(cancellationToken);
    }
}
