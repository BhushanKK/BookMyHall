using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorRepository(BookMyHallDbContext context) : IVendorRepository
{
    private IQueryable<Vendor> VendorQuery()
    {
        return context.Vendors
            .Where(x =>
                !x.IsDeleted &&
                x.UserId != null &&
                context.Users.Any(u =>
                    u.UserId == x.UserId &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.UserRoles.Any(ur =>
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.Role.RoleName == RoleConstants.Vendor)));
    }

    public async Task AddAsync(Vendor vendor, CancellationToken cancellationToken = default)
        => await context.Vendors.AddAsync(vendor, cancellationToken);

    public Task UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        context.Vendors.Update(vendor);
        return Task.CompletedTask;
    }

    public async Task<Vendor?> GetByIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
        => await VendorQuery().FirstOrDefaultAsync(x => x.VendorId == vendorId, cancellationToken);

    public async Task<Vendor?> GetByBusinessNameAsync(string businessName, CancellationToken cancellationToken = default)
        => await VendorQuery().FirstOrDefaultAsync(x => x.BusinessName == businessName, cancellationToken);

    public async Task<Vendor?> GetByBusinessNameIncludingDeletedAsync(
        string businessName,
        CancellationToken cancellationToken = default)
        => await context.Vendors
            .IgnoreQueryFilters()
            .Where(x =>
                x.BusinessName == businessName &&
                x.UserId != null &&
                context.Users.Any(u =>
                    u.UserId == x.UserId &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.UserRoles.Any(ur =>
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.Role.RoleName == "Vendor")))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PaginatedResult<VendorListView>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = from vendor in VendorQuery()
                    join user in context.Users
                    on vendor.UserId equals user.UserId
                    select new VendorListView
                    {
                        VendorId = vendor.VendorId,
                        UserId = vendor.UserId,
                        VendorName = user.FullName,
                        BusinessName = vendor.BusinessName,
                        DisplayName = vendor.DisplayName,
                        Description = vendor.Description,
                        ContactPersonName = vendor.ContactPersonName,
                        Email = vendor.Email,
                        MobileNumber = vendor.MobileNumber,
                        AlternateMobileNumber = vendor.AlternateMobileNumber,
                        WebsiteUrl = vendor.WebsiteUrl,
                        YoutubeUrl = vendor.YoutubeUrl,
                        InstagramUrl = vendor.InstagramUrl,
                        AddressLine1 = vendor.AddressLine1,
                        AddressLine2 = vendor.AddressLine2,
                        Pincode = vendor.Pincode,
                        Latitude = vendor.Latitude,
                        Longitude = vendor.Longitude,
                        EstablishedYear = vendor.EstablishedYear,
                        IsVerified = vendor.IsVerified,
                        IsActive = vendor.IsActive,
                        Rating = vendor.Rating,
                        ReviewCount = vendor.ReviewCount,
                    };

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            var pattern = $"%{searchText}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.BusinessName, pattern) ||
                EF.Functions.ILike(x.VendorName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "vendorname" => request.SortDescending
                ? query.OrderByDescending(x => x.VendorName)
                : query.OrderBy(x => x.VendorName),

            "businessname" => request.SortDescending
                ? query.OrderByDescending(x => x.BusinessName)
                : query.OrderBy(x => x.BusinessName),

            "displayname" => request.SortDescending
                ? query.OrderByDescending(x => x.DisplayName)
                : query.OrderBy(x => x.DisplayName),

            "rating" => request.SortDescending
                ? query.OrderByDescending(x => x.Rating)
                : query.OrderBy(x => x.Rating),

            "reviewcount" => request.SortDescending
                ? query.OrderByDescending(x => x.ReviewCount)
                : query.OrderBy(x => x.ReviewCount),

            _ => query.OrderBy(x => x.BusinessName)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorListView>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
    string? searchTerm,
    int limit = 30,
    CancellationToken cancellationToken = default)
    {
        var query =
            from vendor in VendorQuery()
            join user in context.Users
            on vendor.UserId equals user.UserId
            select new
            {
                vendor.VendorId,
                VendorName =
                    vendor.BusinessName + " - " +
                    (user.FirstName + " " +
                    (user.MiddleName ?? "") + " " +
                    (user.LastName ?? "")).Trim()
            };
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchText = searchTerm.Trim();
            var pattern = $"%{searchText}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.BusinessName, pattern) ||
                EF.Functions.ILike(x.VendorName, pattern));
        }

        return await query
            .OrderBy(x => x.BusinessName)
            .ThenBy(x => x.VendorName)
            .ThenBy(x => x.VendorId)
            .Take(Math.Clamp(limit, 1, 30))
            .Select(x => new AutoCompleteItem(
                x.VendorId,
                x.BusinessName))
            .ToListAsync(cancellationToken);
    }
}