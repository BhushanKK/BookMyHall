using MediatR;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
namespace BookMyHall.Api.Endpoints.Master;

public static class LocationLookupEndpoints
{
    public static void MapLocationLookupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/location-lookup")
            .WithTags("Location Lookup")
            .RequireAuthorization();


        group.MapGet("/countries", async (IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCountriesQuery(),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetLocationLookUpCountries")
        .WithSummary("Get Countries")
        .WithDescription("Retrieves all active countries.")
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>>(StatusCodes.Status200OK);


        group.MapGet("/countries/{countryId:guid}/states",async (Guid countryId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetStatesQuery(countryId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetLocationLookUpStatesByCountry")
        .WithSummary("Get States By Country")
        .WithDescription("Retrieves all active states for the specified country.")
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>>(StatusCodes.Status400BadRequest);

   

        group.MapGet("/states/{stateId:guid}/districts",async(Guid stateId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetDistrictsQuery(stateId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetLocationLookUpDistrictsByState")
        .WithSummary("Get Districts By State")
        .WithDescription("Retrieves all active districts for the specified state.")
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>>(StatusCodes.Status400BadRequest);

       

        group.MapGet("/districts/{districtId:guid}/cities",async(Guid districtId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCitiesQuery(districtId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetLocationLookUpCitiesByDistrict")
        .WithSummary("Get Cities By District")
        .WithDescription("Retrieves all active cities for the specified district.")
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>>(StatusCodes.Status400BadRequest);

    

        group.MapGet("/cities/{cityId:guid}/areas", async (Guid cityId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send( new GetAreasQuery(cityId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetLocationLookUpAreasByCity")
        .WithSummary("Get Areas By City")
        .WithDescription("Retrieves all active areas for the specified city.")
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>>(StatusCodes.Status400BadRequest);

    }
}