using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Master.LocationLookup.Queries;

public sealed record GetCountriesQuery
    : IRequest<ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>>;


public sealed record GetStatesQuery(Guid CountryId)
    : IRequest<ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>>;


public sealed record GetDistrictsQuery(Guid StateId)
    : IRequest<ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>>;


public sealed record GetCitiesQuery(Guid DistrictId)
    : IRequest<ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>>;


public sealed record GetAreasQuery(Guid CityId)
    : IRequest<ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>>;