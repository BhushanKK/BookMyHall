using MediatR;

using System.Net;

using AutoMapper;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetStateByIdQueryHandler(
    IStateRepository stateRepository,
    IMapper mapper, ICacheService cacheService,
    IMessageHelper messageHelper) : IRequestHandler<GetStateByIdQuery, ApiResponse<State>>
{
    public async Task<ApiResponse<State>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.States}:{request.StateId}";
        var cachedState = await cacheService.GetAsync<State>(cacheKey, cancellationToken);
        if (cachedState is not null)
        {
            return ApiResponse<State>.SuccessResponse(cachedState, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.State), HttpStatusCode.OK);
        }
        var state = await stateRepository.GetByIdAsync(request.StateId, cancellationToken);

        if (state is null)
        {
            return ApiResponse<State>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.State), HttpStatusCode.NotFound);
        }
        var response = mapper.Map<State>(state);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<State>.SuccessResponse(
            mapper.Map<State>(state),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.State), HttpStatusCode.OK);
    }
}