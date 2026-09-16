using BookMyHall.Domain.Dtos;
namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface ILocationLookupRepository
{
    Task<IReadOnlyList<LocationLookupDto.CountryLookupDto>>GetCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationLookupDto.StateLookupDto>>GetStatesAsync(Guid countryId,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>GetDistrictsAsync(Guid stateId,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationLookupDto.CityLookupDto>>GetCitiesAsync(Guid districtId,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationLookupDto.AreaLookupDto>>GetAreasAsync(Guid cityId, CancellationToken cancellationToken = default);
}