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

public sealed class CreateFacilityCommandHandler(
    IFacilityRepository facilityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateFacilityCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateFacilityCommand request,CancellationToken cancellationToken)
    {
        var existingFacility = await facilityRepository.GetByFacilityNameAsync(request.FacilityName,cancellationToken);

        if (existingFacility is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Facility),
                HttpStatusCode.BadRequest);
        }

        var facility = mapper.Map<Facility>(request);
        facility.FacilityId = Guid.NewGuid();
        facility.IsActive = true;
        await facilityRepository.AddAsync(facility, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(facility.FacilityId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.Created);
    }
}