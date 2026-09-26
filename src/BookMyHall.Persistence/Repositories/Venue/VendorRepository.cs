using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorRepository(BookMyHallDbContext context) : IVendorRepository
{
    public async Task AddAsync(Vendors vendor, CancellationToken cancellationToken = default)
    {
        await context.Vendors.AddAsync(vendor, cancellationToken);
    }

    public Task UpdateAsync(Vendors vendor, CancellationToken cancellationToken = default)
    {
        context.Vendors.Update(vendor);
        return Task.CompletedTask;
    }

    public async Task<Vendors?> GetByIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await context.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.VendorId == vendorId,cancellationToken);
    }

    public async Task<Vendors?> GetByBusinessNameAsync(string businessName, CancellationToken cancellationToken = default)
    {
        return await context.Vendors.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessName == businessName && !x.IsDeleted,cancellationToken);
    }

    public async Task<Vendors?> GetByBusinessNameIncludingDeletedAsync(string businessName, CancellationToken cancellationToken = default)
    {
        return await context.Vendors
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.BusinessName == businessName, cancellationToken);
    }

    public async Task<PaginatedResult<Vendors>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = context.Vendors
            .AsNoTracking()
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            query = query.Where(x =>
                x.BusinessName.Contains(searchText) ||
                (x.DisplayName != null && x.DisplayName.Contains(searchText)) ||
                (x.ContactPersonName != null && x.ContactPersonName.Contains(searchText)) ||
                (x.MobileNumber != null && x.MobileNumber.Contains(searchText)) ||
                (x.Email != null && x.Email.Contains(searchText)));
        }

        var totalCounts = await query.CountAsync(
            cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "businessname" =>
                request.SortDescending
                    ? query.OrderByDescending(x => x.BusinessName)
                    : query.OrderBy(x => x.BusinessName),

            "displayname" =>
                request.SortDescending
                    ? query.OrderByDescending(x => x.DisplayName)
                    : query.OrderBy(x => x.DisplayName),

            "rating" =>
                request.SortDescending
                    ? query.OrderByDescending(x => x.Rating)
                    : query.OrderBy(x => x.Rating),

            "reviewcount" =>
                request.SortDescending
                    ? query.OrderByDescending(x => x.ReviewCount)
                    : query.OrderBy(x => x.ReviewCount),

            _ =>
                query.OrderBy(x => x.BusinessName)
        };

        var items = await query
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Vendors>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCounts
        };
    }
}