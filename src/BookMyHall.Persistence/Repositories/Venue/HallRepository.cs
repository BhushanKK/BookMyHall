using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Persistence.Repositories;

public sealed class HallRepository(BookMyHallDbContext context) : IHallRepository
{
    public async Task AddAsync(Hall hall, CancellationToken cancellationToken = default)
        => await context.Halls.AddAsync(hall, cancellationToken);

    public Task UpdateAsync(Hall hall, CancellationToken cancellationToken = default)
    {
        context.Halls.Update(hall);
        return Task.CompletedTask;
    }

    public async Task<Hall?> GetByIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        => await context.Halls
        .Where(x => x.IsDeleted == false)
        .AsNoTracking().FirstOrDefaultAsync(x => x.HallId == hallId, cancellationToken);
    public async Task<Hall?> GetByHallNameAndAreaAsync(
    string hallName,
    Guid areaId,
    CancellationToken cancellationToken = default)
    => await context.Halls
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x =>
                x.HallName == hallName &&
                x.AreaId == areaId,
            cancellationToken);
    public async Task<PaginatedResult<HallListView>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        // =============================================================
        // Query Hall List View
        // =============================================================
        var query = context.HallListViews
            .AsNoTracking()
            .AsQueryable();

        // =============================================================
        // Search
        // =============================================================
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var searchPattern = $"%{search}%";

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.HallName,
                    searchPattern)

                || (
                    x.Description != null &&
                    EF.Functions.ILike(
                        x.Description,
                        searchPattern)
                )

                || (
                    x.HallOwnerName != null &&
                    EF.Functions.ILike(
                        x.HallOwnerName,
                        searchPattern)
                )

                || (
                    x.HallCategoryName != null &&
                    EF.Functions.ILike(
                        x.HallCategoryName,
                        searchPattern)
                )

                || (
                    x.CancellationPolicyName != null &&
                    EF.Functions.ILike(
                        x.CancellationPolicyName,
                        searchPattern)
                )

                || (
                    x.AreaName != null &&
                    EF.Functions.ILike(
                        x.AreaName,
                        searchPattern)
                )

                || (
                    x.CityName != null &&
                    EF.Functions.ILike(
                        x.CityName,
                        searchPattern)
                )

                || (
                    x.DistrictName != null &&
                    EF.Functions.ILike(
                        x.DistrictName,
                        searchPattern)
                )

                || (
                    x.StateName != null &&
                    EF.Functions.ILike(
                        x.StateName,
                        searchPattern)
                )

                || (
                    x.CountryName != null &&
                    EF.Functions.ILike(
                        x.CountryName,
                        searchPattern)
                )

                || (
                    x.AddressLine1 != null &&
                    EF.Functions.ILike(
                        x.AddressLine1,
                        searchPattern)
                )

                || (
                    x.AddressLine2 != null &&
                    EF.Functions.ILike(
                        x.AddressLine2,
                        searchPattern)
                )

                || (
                    x.Pincode != null &&
                    EF.Functions.ILike(
                        x.Pincode,
                        searchPattern)
                )

                || (
                    x.MobileNumber != null &&
                    EF.Functions.ILike(
                        x.MobileNumber,
                        searchPattern)
                )

                || (
                    x.EmailAddress != null &&
                    EF.Functions.ILike(
                        x.EmailAddress,
                        searchPattern)
                )

                || (
                    x.ApprovalStatus != null &&
                    EF.Functions.ILike(
                        x.ApprovalStatus,
                        searchPattern)
                )

                || (
                    x.VerificationStatus != null &&
                    EF.Functions.ILike(
                        x.VerificationStatus,
                        searchPattern)
                )
            );
        }

        // =============================================================
        // Total Records
        // =============================================================
        var totalCount = await query.CountAsync(
            cancellationToken);

        // =============================================================
        // Sorting
        // =============================================================
        query = query.OrderBy(x => x.HallName);

        // =============================================================
        // Pagination
        // =============================================================
        var items = await query
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // =============================================================
        // Result
        // =============================================================
        return new PaginatedResult<HallListView>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}