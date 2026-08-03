using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetStateQueryHandler(
    IStateRepository stateRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetStateQuery, ApiResponse<PaginatedResult<State>>>
{
    public async Task<ApiResponse<PaginatedResult<State>>> Handle(GetStateQuery request,CancellationToken cancellationToken)
    {
        var result = await stateRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResult<State>
        {
            Items = mapper.Map<IReadOnlyList<State>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<State>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.OK);
    }
}