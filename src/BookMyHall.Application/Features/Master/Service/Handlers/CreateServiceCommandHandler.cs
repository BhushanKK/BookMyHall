using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateServiceCommandHandler(
    IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateServiceCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateServiceCommand request,CancellationToken cancellationToken)
    {
        var existingService = await serviceRepository.GetByServiceNameAsync(request.ServiceName,cancellationToken);
        if (existingService is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Service),
                HttpStatusCode.BadRequest);
        }

        var service = mapper.Map<Service>(request);
        service.ServiceId = Guid.NewGuid();
        service.IsActive = true;
        await serviceRepository.AddAsync(service, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(service.ServiceId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.Created);
    }
}