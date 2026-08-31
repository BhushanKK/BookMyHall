using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetStateByStateNameQueryHandler(
    IStateRepository stateRepository,IMapper mapper,ICacheService cacheService,
    IMessageHelper messageHelper): IRequestHandler<GetStateByStateNameQuery, ApiResponse<StateDto>>
{
    public async Task<ApiResponse<StateDto>> Handle(GetStateByStateNameQuery request,CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.StateByName}:{request.StateName.Trim().ToUpperInvariant()}";
        var cachedStateName = await cacheService.GetAsync<StateDto>(cacheKey, cancellationToken);
        if (cachedStateName is not null)
        {
            return ApiResponse<StateDto>.SuccessResponse(cachedStateName,
                messageHelper.RetrievedEntity(ResourceNames.Entities,
                    EntityKeys.State), HttpStatusCode.OK);
        }
        var state = await stateRepository.GetByStateNameAsync(request.StateName,cancellationToken);

        if (state is null)
        {
            return ApiResponse<StateDto>.FailureResponse(messageHelper.NotFound(EntityKeys.State),HttpStatusCode.NotFound);
        }

        var response = mapper.Map<StateDto>(state);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<StateDto>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.State),HttpStatusCode.OK);
    }
}