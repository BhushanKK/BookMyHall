using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServiceByIdQueryHandler(IServiceRepository serviceRepository,
    IMessageHelper messageHelper,IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetServiceByIdQuery, ApiResponse<Service>>
{
    public async Task<ApiResponse<Service>> Handle(GetServiceByIdQuery request,CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Services}:{request.ServiceId}";
        var cachedPaymentMode = await cacheService.GetAsync<Service>(cacheKey, cancellationToken);

        if (cachedPaymentMode is not null)
        {
            return ApiResponse<Service>.SuccessResponse
            (
                cachedPaymentMode,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Service),
                HttpStatusCode.OK
            );
        }
        var service = await serviceRepository.GetByIdAsync(request.ServiceId,cancellationToken);
        if (service is null)
        {
            return ApiResponse<Service>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Service),
                HttpStatusCode.NotFound);
        }
          var response = mapper.Map<Service>(service);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
       
        return ApiResponse<Service>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}