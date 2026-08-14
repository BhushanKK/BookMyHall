using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlockByIdQueryHandler(
    IHallBlockRepository hallBlockRepository,
    IMapper mapper, IMessageHelper messageHelper)
    : IRequestHandler<GetHallBlockByIdQuery, ApiResponse<HallBlockDto>>
{
    public async Task<ApiResponse<HallBlockDto>> Handle(GetHallBlockByIdQuery request, CancellationToken cancellationToken)
    {
        var hallBlock = await hallBlockRepository.GetByIdAsync(request.HallBlockId, cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.HallBlock), HttpStatusCode.NotFound);
        }

        return ApiResponse<HallBlockDto>.SuccessResponse(mapper.Map<HallBlockDto>(hallBlock),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallBlock), HttpStatusCode.OK);
    }
}