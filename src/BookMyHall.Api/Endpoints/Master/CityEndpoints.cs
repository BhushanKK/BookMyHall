using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class CityEndpoints
{
    public static void MapCityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cities")
            .WithTags("Cities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateCityAsync)
            .WithName("CreateCity")
            .WithSummary("Create City")
            .WithDescription("Creates a new city.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{cityId:guid}", UpdateCityAsync)
            .WithName("UpdateCity")
            .WithSummary("Update City")
            .WithDescription("Updates an existing city.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{cityId:guid}", DeleteCityAsync)
            .WithName("DeleteCity")
            .WithSummary("Delete City")
            .WithDescription("Soft deletes a city.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{cityId:guid}", GetCityByIdAsync)
            .WithName("GetCityById")
            .WithSummary("Get City By Id")
            .WithDescription("Retrieves a city by its unique identifier.")
            .Produces<ApiResponse<CityDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CityDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllCities", GetCitiesAsync)
            .WithName("GetCities")
            .WithSummary("Get Cities")
            .WithDescription("Retrieves a paginated list of cities.")
            .Produces<ApiResponse<PaginatedResult<CityDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<CityDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateCityAsync(
        CreateCityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> UpdateCityAsync(
        Guid cityId,
        UpdateCityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.CityId = cityId;
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> DeleteCityAsync(
        Guid cityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteCityCommand(cityId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetCityByIdAsync(
        Guid cityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetCityByIdQuery(cityId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetCitiesAsync(
        GetCitiesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }
}