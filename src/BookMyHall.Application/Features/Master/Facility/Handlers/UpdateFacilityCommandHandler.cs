using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateFacilityCommandHandler(
    IFacilityRepository facilityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateFacilityCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateFacilityCommand, ApiResponse<FacilityDto>>
{
    public async Task<ApiResponse<FacilityDto>> Handle(UpdateFacilityCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<FacilityDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var facility = await facilityRepository.GetByIdAsync(request.FacilityId,cancellationToken);
        if (facility is null)
        {
            return ApiResponse<FacilityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Facility),
                HttpStatusCode.NotFound);
        }

        var existingFacility = await facilityRepository.GetByFacilityNameAsync(request.FacilityName,cancellationToken);
        if (existingFacility is not null && existingFacility.FacilityId != request.FacilityId)
        {
            return ApiResponse<FacilityDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Facility),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, facility);
        await facilityRepository.UpdateAsync(facility,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<FacilityDto>.SuccessResponse(
            mapper.Map<FacilityDto>(facility),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.OK);
    }
}