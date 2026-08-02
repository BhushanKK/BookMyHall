using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServiceByIdQueryHandler(
    IServiceRepository serviceRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetServiceByIdQuery, ApiResponse<Service>>
{
    public async Task<ApiResponse<Service>> Handle(GetServiceByIdQuery request,CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId,cancellationToken);
        if (service is null)
        {
            return ApiResponse<Service>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Service),
                HttpStatusCode.NotFound);
        }
        return ApiResponse<Service>.SuccessResponse(
            mapper.Map<Service>(service),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}