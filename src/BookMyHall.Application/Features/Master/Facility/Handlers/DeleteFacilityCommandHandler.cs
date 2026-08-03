using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteFacilityCommandHandler(
    IFacilityRepository facilityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteFacilityCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteFacilityCommand request,CancellationToken cancellationToken)
    {
        var facility = await facilityRepository.GetByIdAsync(request.FacilityId,cancellationToken);

        if (facility is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Facility),
                HttpStatusCode.NotFound);
        }

        facility.IsActive = false;
        await facilityRepository.UpdateAsync(facility, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.OK);
    }
}