using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetNearbyHallsQueryHandler(
    IHallRepository hallRepository,
    IR2StorageService storageService,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetNearbyHallsQuery,
        ApiResponse<PaginatedResult<NearbyHallView>>>
{
    public async Task<ApiResponse<PaginatedResult<NearbyHallView>>> Handle(
        GetNearbyHallsQuery request,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // VALIDATE LATITUDE
        // ============================================================
        // Latitude is optional.
        //
        // Device-location search:
        //     Latitude + Longitude are provided.
        //
        // Manual-location search:
        //     Latitude + Longitude can both be null.
        // ============================================================

        if (request.Latitude.HasValue &&
            (request.Latitude.Value < -90 ||
             request.Latitude.Value > 90))
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Latitude must be between -90 and 90.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE LONGITUDE
        // ============================================================

        if (request.Longitude.HasValue &&
            (request.Longitude.Value < -180 ||
             request.Longitude.Value > 180))
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Longitude must be between -180 and 180.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE COORDINATE PAIR
        // ============================================================
        // Either both coordinates must be supplied or both must be null.
        // This prevents invalid requests such as:
        //
        // Latitude = 19.97
        // Longitude = null
        // ============================================================

        if (request.Latitude.HasValue != request.Longitude.HasValue)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Latitude and longitude must either both be provided or both be omitted.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE RADIUS
        // ============================================================
        // Radius 0 means unlimited distance.
        // Radius > 0 applies the distance filter.
        // Negative radius values are invalid.
        //
        // For manual location filtering, radius can still be used
        // when coordinates are not available.
        // ============================================================

        if (request.RadiusKm < 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Radius cannot be negative.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE PAGE NUMBER
        // ============================================================

        if (request.PageNumber <= 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Page number must be greater than 0.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE PAGE SIZE
        // ============================================================

        if (request.PageSize <= 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Page size must be greater than 0.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE SEARCH CRITERIA
        // ============================================================
        // At least one search method must be available:
        //
        // 1. Device location
        // OR
        // 2. Manual location filter
        //
        // We do not allow a completely empty nearby-halls search.
        // ============================================================

        var hasDeviceLocation =
            request.Latitude.HasValue &&
            request.Longitude.HasValue;

        var hasManualLocationFilter =
            request.StateId.HasValue ||
            request.DistrictId.HasValue ||
            request.CityId.HasValue ||
            request.AreaId.HasValue;

        if (!hasDeviceLocation && !hasManualLocationFilter)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Please provide your current location or select a location manually.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // CREATE PAGINATION REQUEST
        // ============================================================

        var paginationRequest = new PaginationRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // ============================================================
        // GET HALLS
        // ============================================================
        // Coordinates may be null for manual location search.
        // ============================================================

        var result = await hallRepository.GetNearbyAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            request.StateId,
            request.DistrictId,
            request.CityId,
            request.AreaId,
            paginationRequest,
            cancellationToken);

        // ============================================================
        // GENERATE R2 PRESIGNED URL FOR COVER IMAGE
        // ============================================================

        foreach (var hall in result.Items)
        {
            if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
            {
                hall.CoverImageUrl =
                    await storageService.GetPreSignedUrlAsync(
                        hall.CoverImageUrl,
                        TimeSpan.FromDays(6).Add(
                            TimeSpan.FromHours(23)),
                        cancellationToken);
            }
        }

        // ============================================================
        // RETURN RESPONSE
        // ============================================================

        return ApiResponse<PaginatedResult<NearbyHallView>>.SuccessResponse(
            result,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}