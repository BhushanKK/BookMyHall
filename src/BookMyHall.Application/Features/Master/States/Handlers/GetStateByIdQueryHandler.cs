using MediatR;

using System.Net;

using AutoMapper;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Master;

public sealed class GetStateByIdQueryHandler(
    IStateRepository stateRepository,
    IMapper mapper,
    IMessageHelper messageHelper): IRequestHandler<GetStateByIdQuery, ApiResponse<State>>
{
    public async Task<ApiResponse<State>> Handle(GetStateByIdQuery request,CancellationToken cancellationToken)
    {
        var state = await stateRepository.GetByIdAsync(request.StateId,cancellationToken);

        if (state is null)
        {
            return ApiResponse<State>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.NotFound);
        }

        return ApiResponse<State>.SuccessResponse(
            mapper.Map<State>(state),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.OK);
    }
}