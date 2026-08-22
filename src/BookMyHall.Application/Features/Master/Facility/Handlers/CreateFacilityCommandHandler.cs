using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateFacilityCommandHandler(
    IFacilityRepository facilityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateFacilityCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateFacilityCommand, ApiResponse<FacilityDto>>
{
    public async Task<ApiResponse<FacilityDto>> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<FacilityDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var facility = mapper.Map<Facility>(request);
        facility.FacilityId = Guid.NewGuid();
        facility.IsActive = true;

        try
        {
            await facilityRepository.AddAsync(facility, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<FacilityDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Facility), HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.EventCategory}:", cancellationToken);

        return ApiResponse<FacilityDto>.SuccessResponse(
            mapper.Map<FacilityDto>(facility),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Facility), HttpStatusCode.Created);
    }
}