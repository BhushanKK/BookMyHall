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

    public Task<PaginatedResult<VendorListView>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
        => GetAllAsync(request, null, cancellationToken);

    public async Task<PaginatedResult<VendorListView>> GetAllAsync(
        PaginationRequest request,
        Guid? areaId,
        CancellationToken cancellationToken = default)
    {
        var query = from vendor in VendorQuery()
                    join user in context.Users
                    on vendor.UserId equals user.UserId
                    select new
                    {
                        VendorId = vendor.VendorId,
                        UserId = vendor.UserId,
                        AreaId = context.VendorServiceAreas
                            .Where(x =>
                                x.VendorId == vendor.VendorId &&
                                x.IsActive &&
                                !x.IsDeleted &&
                                (!areaId.HasValue || x.AreaId == areaId.Value))
                            .OrderByDescending(x => x.IsPrimary)
                            .Select(x => x.AreaId)
                            .FirstOrDefault(),
                        VendorName = user.FirstName +
                            (string.IsNullOrWhiteSpace(user.MiddleName) ? string.Empty : " " + user.MiddleName) +
                            (string.IsNullOrWhiteSpace(user.LastName) ? string.Empty : " " + user.LastName),
                        BusinessName = vendor.BusinessName,
                        LogoUrl = vendor.LogoUrl,
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

        if (areaId.HasValue)
        {
            var vendorIdsInArea = context.VendorServiceAreas
                .AsNoTracking()
                .Where(x => x.AreaId == areaId.Value && x.IsActive && !x.IsDeleted)
                .Select(x => x.VendorId)
                .Distinct();

            query = query.Where(x => vendorIdsInArea.Contains(x.VendorId));
        }

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

        var mappedItems = items.Select(x => new VendorListView
        {
            VendorId = x.VendorId,
            UserId = x.UserId,
            AreaId = x.AreaId,
            VendorName = string.IsNullOrWhiteSpace(x.VendorName) ? x.BusinessName : x.VendorName,
            BusinessName = x.BusinessName,
            LogoUrl = string.IsNullOrWhiteSpace(x.LogoUrl) ? null : $"/api/vendors/{x.VendorId}/logo/content",
            DisplayName = x.DisplayName,
            Description = x.Description,
            ContactPersonName = x.ContactPersonName,
            Email = x.Email,
            MobileNumber = x.MobileNumber,
            AlternateMobileNumber = x.AlternateMobileNumber,
            WebsiteUrl = x.WebsiteUrl,
            YoutubeUrl = x.YoutubeUrl,
            InstagramUrl = x.InstagramUrl,
            AddressLine1 = x.AddressLine1,
            AddressLine2 = x.AddressLine2,
            Pincode = x.Pincode,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            EstablishedYear = x.EstablishedYear,
            IsVerified = x.IsVerified,
            IsActive = x.IsActive,
            Rating = x.Rating,
            ReviewCount = x.ReviewCount,
        }).ToList();

        return new PaginatedResult<VendorListView>
        {
            Items = mappedItems,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm,
        int limit = 30,
        CancellationToken cancellationToken = default)
        => GetAutoCompleteAsync(searchTerm, null, limit, cancellationToken);

    public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm,
        Guid? areaId,
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
                VendorName = user.FirstName +
                    (string.IsNullOrWhiteSpace(user.MiddleName) ? string.Empty : " " + user.MiddleName) +
                    (string.IsNullOrWhiteSpace(user.LastName) ? string.Empty : " " + user.LastName)
            };

        if (areaId.HasValue)
        {
            var vendorIdsInArea = context.VendorServiceAreas
                .AsNoTracking()
                .Where(x => x.AreaId == areaId.Value && x.IsActive && !x.IsDeleted)
                .Select(x => x.VendorId)
                .Distinct();

            query = query.Where(x => vendorIdsInArea.Contains(x.VendorId));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchText = searchTerm.Trim();
            var pattern = $"%{searchText}%";
            query = query.Where(x => EF.Functions.ILike(x.VendorName, pattern));
        }

        var items = await query
            .OrderBy(x => x.VendorName)
            .ThenBy(x => x.VendorId)
            .Take(Math.Clamp(limit, 1, 30))
            .ToListAsync(cancellationToken);

        return items
            .GroupBy(x => x.VendorName)
            .Select(g => g.First())
            .Select(x => new AutoCompleteItem(x.VendorId, x.VendorName))
            .Take(Math.Clamp(limit, 1, 30))
            .ToList();
    }

    public async Task<IReadOnlyList<AutoCompleteItem>> GetBusinessAutoCompleteAsync(
        Guid vendorId,
        string? searchTerm,
        Guid? areaId,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = await VendorQuery()
            .Where(x => x.VendorId == vendorId)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (!userId.HasValue)
        {
            return [];
        }

        var query = VendorQuery()
            .Where(x => x.UserId == userId.Value);

        if (areaId.HasValue)
        {
            var vendorIdsInArea = context.VendorServiceAreas
                .AsNoTracking()
                .Where(x => x.AreaId == areaId.Value && x.IsActive && !x.IsDeleted)
                .Select(x => x.VendorId)
                .Distinct();

            query = query.Where(x => vendorIdsInArea.Contains(x.VendorId));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.BusinessName, pattern) ||
                (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
        }

        return await query
            .OrderBy(x => x.DisplayName ?? x.BusinessName)
            .ThenBy(x => x.VendorId)
            .Take(Math.Clamp(limit, 1, 30))
            .Select(x => new AutoCompleteItem(x.VendorId, x.DisplayName ?? x.BusinessName))
            .ToListAsync(cancellationToken);
    }
}
