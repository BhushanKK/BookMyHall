using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetStateByStateNameQueryHandler(
    IStateRepository stateRepository,IMapper mapper,
    IMessageHelper messageHelper): IRequestHandler<GetStateByStateNameQuery, ApiResponse<StateDto>>
{
    public async Task<ApiResponse<StateDto>> Handle(GetStateByStateNameQuery request,CancellationToken cancellationToken)
    {
        var state = await stateRepository.GetByStateNameAsync(request.StateName,cancellationToken);

        if (state is null)
        {
            return ApiResponse<StateDto>.FailureResponse(messageHelper.NotFound(EntityKeys.State),HttpStatusCode.NotFound);
        }

        var response = mapper.Map<StateDto>(state);
        return ApiResponse<StateDto>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.State),HttpStatusCode.OK);
    }
}