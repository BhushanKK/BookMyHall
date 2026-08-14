using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImagesByHallIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetHallImagesByHallIdQuery,
        ApiResponse<PaginatedResult<HallImageDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallImageDto>>> Handle(
        GetHallImagesByHallIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await hallImageRepository.GetByHallIdAsync(
            request.HallId,
            request.Pagination,
            cancellationToken);

        if (result.Items is null || result.Items.Count == 0)
        {
            return ApiResponse<PaginatedResult<HallImageDto>>.FailureResponse
            (
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        var mappedResult = new PaginatedResult<HallImageDto>
        {
            Items = mapper.Map<IReadOnlyList<HallImageDto>>(
                result.Items),

            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<HallImageDto>>.SuccessResponse
        (
            mappedResult,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}