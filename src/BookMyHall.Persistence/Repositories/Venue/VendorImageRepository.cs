using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories.Venue;

public sealed class VendorImageRepository(BookMyHallDbContext context) : IVendorImageRepository
{
    public Task<VendorImage?> GetByIdAsync(Guid vendorImageId, CancellationToken cancellationToken = default)
        => context.VendorImages
            .Include(x => x.VendorService).ThenInclude(x => x!.VendorSubCategory).ThenInclude(x => x.VendorCategory)
            .FirstOrDefaultAsync(x => x.VendorImageId == vendorImageId, cancellationToken);

    public async Task<PaginatedResult<VendorImage>> GetByVendorIdAsync(
        Guid vendorId,
        PaginationRequest request,
        CancellationToken cancellationToken = default,
        Guid? vendorServiceId = null,
        Guid? vendorCategoryId = null,
        Guid? vendorSubCategoryId = null)
    {
        var query = context.VendorImages
            .AsNoTracking()
            .Include(x => x.VendorService).ThenInclude(x => x!.VendorSubCategory).ThenInclude(x => x.VendorCategory)
            .Where(x => x.VendorId == vendorId && x.IsActive);

        if (vendorServiceId.HasValue)
        {
            query = query.Where(x => x.VendorServiceId == vendorServiceId);
        }

        if (vendorCategoryId.HasValue)
            query = query.Where(x => x.VendorService != null && x.VendorService.VendorCategoryId == vendorCategoryId);
        if (vendorSubCategoryId.HasValue)
            query = query.Where(x => x.VendorService != null && x.VendorService.VendorSubCategoryId == vendorSubCategoryId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.VendorImageId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorImage>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public Task<VendorImage?> GetCoverImageAsync(Guid vendorId, CancellationToken cancellationToken = default, Guid? vendorServiceId = null)
        => context.VendorImages.AsNoTracking()
            .Include(x => x.VendorService).ThenInclude(x => x!.VendorSubCategory).ThenInclude(x => x.VendorCategory)
            .FirstOrDefaultAsync(x => x.VendorId == vendorId && x.VendorServiceId == vendorServiceId && x.IsCoverImage && x.IsActive, cancellationToken);

    public Task AddAsync(VendorImage vendorImage, CancellationToken cancellationToken = default)
        => context.VendorImages.AddAsync(vendorImage, cancellationToken).AsTask();

    public Task UpdateAsync(VendorImage vendorImage, CancellationToken cancellationToken = default)
    {
        context.VendorImages.Update(vendorImage);
        return Task.CompletedTask;
    }

    public async Task ClearOtherCoverImagesAsync(
        Guid vendorId,
        Guid? exceptVendorImageId,
        CancellationToken cancellationToken = default,
        Guid? vendorServiceId = null)
    {
        var covers = await context.VendorImages
            .Where(x => x.VendorId == vendorId && x.VendorServiceId == vendorServiceId && x.IsCoverImage && x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var cover in covers.Where(x => x.VendorImageId != exceptVendorImageId))
        {
            cover.IsCoverImage = false;
        }
    }
}
