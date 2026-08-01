using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServiceByIdQueryHandler(
    IServiceRepository serviceRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetServiceByIdQuery, ApiResponse<ServiceDto>>
{
    public async Task<ApiResponse<ServiceDto>> Handle(GetServiceByIdQuery request,CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId,cancellationToken);
        if (service is null)
        {
            return ApiResponse<ServiceDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Service),
                HttpStatusCode.NotFound);
        }
        return ApiResponse<ServiceDto>.SuccessResponse(
            mapper.Map<ServiceDto>(service),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}