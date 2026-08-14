using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class CountryEndpoints
{
    public static void MapCountryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/countries")
            .WithTags("Country")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateCountryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("CreateCountry")
        .WithSummary("Create Country")
        .WithDescription("Creates a new country.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{countryId:guid}", async (
            Guid countryId,
            UpdateCountryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.CountryId = countryId;

            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("UpdateCountry")
        .WithSummary("Update Country")
        .WithDescription("Updates an existing country.")
        .Produces<ApiResponse<CountryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{countryId:guid}", async (
            Guid countryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteCountryCommand(countryId),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("DeleteCountry")
        .WithSummary("Delete Country")
        .WithDescription("Deletes an existing country.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{countryId:guid}", async (
            Guid countryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetCountryByIdQuery(countryId),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("GetByIdCountry")
        .WithSummary("Get Country By Id")
        .WithDescription("Returns a country by its identifier.")
        .Produces<ApiResponse<CountryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetCountriesQuery(request),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("GetCountries")
        .WithSummary("Get Countries")
        .WithDescription("Returns a paginated list of countries.")
        .Produces<ApiResponse<PaginatedResponse<CountryDto>>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}