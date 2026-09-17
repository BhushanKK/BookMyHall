using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Dtos;
using BookMyHall.Persistence.Context;
namespace BookMyHall.Infrastructure.Persistence.Repositories;
public sealed class LocationLookupRepository(BookMyHallDbContext context): ILocationLookupRepository
{
    public async Task<IReadOnlyList<LocationLookupDto.CountryLookupDto>>GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Countries.AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted==false)
            .OrderBy(x => x.CountryName)
            .ThenBy(x=>x.CountryId)
            .Select(x => new LocationLookupDto.CountryLookupDto
            {
                CountryId = x.CountryId,
                CountryName = x.CountryName,
                CountryCode = x.CountryCode,
                PhoneCode = x.PhoneCode,
                CurrencyCode = x.CurrencyCode
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationLookupDto.StateLookupDto>>GetStatesAsync(Guid countryId,CancellationToken cancellationToken = default)
    {
        return await (from state in context.States.AsNoTracking()
            join country in context.Countries.AsNoTracking()
            on state.CountryId equals country.CountryId
            where state.CountryId == countryId &&
                state.IsActive && !state.IsDeleted &&
                country.IsActive && !country.IsDeleted
            orderby state.StateName, state.StateId
            select new LocationLookupDto.StateLookupDto
            {
                StateId = state.StateId,
                CountryId = state.CountryId,
                StateName = state.StateName,
                StateCode = state.StateCode
            }
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>GetDistrictsAsync(Guid stateId,CancellationToken cancellationToken = default)
    {
        return await (from district in context.Districts.AsNoTracking()
            join state in context.States.AsNoTracking()
            on district.StateId equals state.StateId
            where  district.StateId == stateId &&
            district.IsActive &&!district.IsDeleted &&
            state.IsActive && !state.IsDeleted
            orderby district.DistrictName, district.DistrictId
            select new LocationLookupDto.DistrictLookupDto
            {
                DistrictId = district.DistrictId,
                StateId = district.StateId,
                DistrictName = district.DistrictName
            }
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationLookupDto.CityLookupDto>>GetCitiesAsync(Guid districtId,CancellationToken cancellationToken = default)
    {
        return await (
            from city in context.Cities.AsNoTracking()
            join district in context.Districts.AsNoTracking()
            on city.DistrictId equals district.DistrictId
            where city.DistrictId == districtId &&
            city.IsActive && !city.IsDeleted &&
            district.IsActive && !district.IsDeleted
            orderby city.CityName, city.CityId
            select new LocationLookupDto.CityLookupDto
            {
                CityId = city.CityId,
                DistrictId = city.DistrictId,
                CityName = city.CityName
            }
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationLookupDto.AreaLookupDto>>GetAreasAsync(Guid cityId,CancellationToken cancellationToken = default)
    {
        return await (from area in context.Areas.AsNoTracking()
            join city in context.Cities.AsNoTracking()
            on area.CityId equals city.CityId
            where area.CityId == cityId && area.IsActive &&
            !area.IsDeleted && city.IsActive && !city.IsDeleted
            orderby area.AreaName, area.AreaId
            select new LocationLookupDto.AreaLookupDto
            {
                AreaId = area.AreaId,
                CityId = area.CityId,
                AreaName = area.AreaName,
                PinCode=area.Pincode
            }
        ).ToListAsync(cancellationToken);
    }
}