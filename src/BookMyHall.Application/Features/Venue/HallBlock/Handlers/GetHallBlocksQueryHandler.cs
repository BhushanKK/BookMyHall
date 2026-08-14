using System.Net;
using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlocksQueryHandler(
    IHallBlockRepository hallBlockRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetHallBlocksQuery,ApiResponse<PaginatedResponse<HallBlockDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<HallBlockDto>>> Handle(GetHallBlocksQuery request,CancellationToken cancellationToken)
    {
        var pagedResult = await hallBlockRepository.GetAllAsync(request.Request,cancellationToken);
        var response = new PaginatedResponse<HallBlockDto>
        {
            Items = mapper.Map<IReadOnlyList<HallBlockDto>>(
                pagedResult.Items),

            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<HallBlockDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallBlock),HttpStatusCode.OK);
    }
}