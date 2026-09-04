using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Dtos;

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
    public async Task<PaginatedResult<HallListDto>> GetAllAsync(
    PaginationRequest request,
    CancellationToken cancellationToken = default)
    {
        var query = context.Halls
            .AsNoTracking()
            .Where(hall => !hall.IsDeleted)

            // Hall Owner
            .LeftJoin(
                context.Users.AsNoTracking(),
                hall => hall.HallOwnerId,
                owner => owner.UserId,
                (hall, owner) => new
                {
                    hall,
                    owner,
                    HallVerificationStatus= hall.VerificationStatus,
                    HallApprovalStatus= hall.ApprovalStatus,
                    MobileNumber= hall!.MobileNumber!
                })

            // Hall Category
            .LeftJoin(
                context.HallCategories.AsNoTracking(),
                x => x.hall.HallCategoryId,
                category => category.HallCategoryId,
                (x, category) => new
                {
                    x.hall,
                    x.owner,
                    category
                })

            // Cancellation Policy
            .LeftJoin(
                context.CancellationPolicies.AsNoTracking(),
                x => x.hall.CancellationPolicyId,
                policy => policy.CancellationPolicyId,
                (x, policy) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    policy
                })

            // Area
            .LeftJoin(
                context.Areas.AsNoTracking(),
                x => x.hall.AreaId,
                area => area.AreaId,
                (x, area) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    x.policy,
                    area
                })

            // City
            .LeftJoin(
                context.Cities.AsNoTracking(),
                x => x.area!.CityId,
                city => city.CityId,
                (x, city) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    x.policy,
                    x.area,
                    city
                })

            // District
            .LeftJoin(
                context.Districts.AsNoTracking(),
                x => x.city!.DistrictId,
                district => district.DistrictId,
                (x, district) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    x.policy,
                    x.area,
                    x.city,
                    district
                })

            // State
            .LeftJoin(
                context.States.AsNoTracking(),
                x => x.district!.StateId,
                state => state.StateId,
                (x, state) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    x.policy,
                    x.area,
                    x.city,
                    x.district,
                    state
                })

            // Country
            .LeftJoin(
                context.Countries.AsNoTracking(),
                x => x.state!.CountryId,
                country => country.CountryId,
                (x, country) => new
                {
                    x.hall,
                    x.owner,
                    x.category,
                    x.policy,
                    x.area,
                    x.city,
                    x.district,
                    x.state,
                    country
                })

            // Cover Image
            .LeftJoin(
                context.HallImages
                    .AsNoTracking()
                    .Where(image =>
                        !image.IsDeleted &&
                        image.IsActive &&
                        image.IsCoverImage),
                x => x.hall.HallId,
                image => image.HallId,
                (x, image) => new HallListDto
                {
                    HallId = x.hall.HallId,

                    HallName = x.hall.HallName,
                    Description = x.hall.Description,

                    HallOwnerName = x.owner != null
                        ? x.owner.FullName
                        : string.Empty,

                    HallCategoryName = x.category != null
                        ? x.category.HallCategoryName
                        : string.Empty,

                    CancellationPolicyName = x.policy != null
                        ? x.policy.PolicyName
                        : null,

                    AreaName = x.area != null
                        ? x.area.AreaName
                        : string.Empty,

                    CityName = x.city != null
                        ? x.city.CityName
                        : null,

                    DistrictName = x.district != null
                        ? x.district.DistrictName
                        : null,

                    StateName = x.state != null
                        ? x.state.StateName
                        : null,

                    CountryName = x.country != null
                        ? x.country.CountryName
                        : null,

                    AddressLine1 = x.hall.AddressLine1,
                    AddressLine2 = x.hall.AddressLine2,
                    Pincode = x.hall.Pincode,

                    MinimumCapacity = x.hall.MinimumCapacity,
                    MaximumCapacity = x.hall.MaximumCapacity,

                    IsActive = x.hall.IsActive,

                    CoverImageUrl = image != null
                        ? image.ImageUrl
                        : null
                });

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var searchPattern = $"%{search}%";

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.HallName,
                    searchPattern)

                || EF.Functions.ILike(
                    x.HallOwnerName,
                    searchPattern)

                || EF.Functions.ILike(
                    x.HallCategoryName,
                    searchPattern)

                || EF.Functions.ILike(
                    x.AreaName,
                    searchPattern)

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
            );
        }

        // Total records
        var totalCount = await query.CountAsync(
            cancellationToken);

        // Pagination
        var items = await query
            .OrderBy(x => x.HallName)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<HallListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}