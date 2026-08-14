using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallCoverImageQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetHallCoverImageQuery, ApiResponse<HallImageDto>>
{
    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallCoverImageQuery request,
        CancellationToken cancellationToken)
    {
        var coverImage = await hallImageRepository.GetCoverImageAsync(request.HallId, cancellationToken);

        if (coverImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<HallImageDto>.SuccessResponse
        (
            mapper.Map<HallImageDto>(coverImage),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}