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
        var query = from hall in context.Halls.AsNoTracking()

            join owner in context.Users.AsNoTracking()
                on hall.HallOwnerId equals owner.UserId

            join category in context.HallCategories.AsNoTracking()
                on hall.HallCategoryId equals category.HallCategoryId

            join policy in context.CancellationPolicies.AsNoTracking()
                on hall.CancellationPolicyId equals policy.CancellationPolicyId
                into policyGroup

            from policy in policyGroup.DefaultIfEmpty()

            join area in context.Areas.AsNoTracking()
                on hall.AreaId equals area.AreaId

            join city in context.Cities.AsNoTracking()
                on area.CityId equals city.CityId

            join district in context.Districts.AsNoTracking()
                on city.DistrictId equals district.DistrictId

            join state in context.States.AsNoTracking()
                on district.StateId equals state.StateId

            join country in context.Countries.AsNoTracking()
                on state.CountryId equals country.CountryId

            where !hall.IsDeleted

            select new HallListDto
            {
                HallId = hall.HallId,

                HallName = hall.HallName,
                Description = hall.Description,

                HallOwnerName = owner.FullName,
                HallCategoryName = category.HallCategoryName,

                CancellationPolicyName =
                    policy != null
                        ? policy.PolicyName
                        : null,

                AreaName = area.AreaName,
                CityName = city.CityName,
                DistrictName = district.DistrictName,
                StateName = state.StateName,
                CountryName = country.CountryName,

                AddressLine1 = hall.AddressLine1,
                AddressLine2 = hall.AddressLine2,
                Pincode = hall.Pincode,

                MinimumCapacity = hall.MinimumCapacity,
                MaximumCapacity = hall.MaximumCapacity,

                IsActive = hall.IsActive
            };

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

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(x => x.HallName)
            .Skip((request.PageNumber - 1) * request.PageSize)
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