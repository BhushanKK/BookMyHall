using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAmenityCommandHandler(
    IAmenityRepository amenityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateAmenityCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateAmenityCommand, ApiResponse<AmenityDto>>
{
    public async Task<ApiResponse<AmenityDto>> Handle(UpdateAmenityCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<AmenityDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var amenity = await amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity is null)
        {
            return ApiResponse<AmenityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Amenity),
                HttpStatusCode.NotFound);
        }

        var existingAmenity = await amenityRepository.GetByAmenityNameAsync(request.AmenityName, cancellationToken);

        if (existingAmenity is not null && existingAmenity.AmenityId != request.AmenityId)
        {
            return ApiResponse<AmenityDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Amenity),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, amenity);
        await amenityRepository.UpdateAsync(amenity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Amenity}:{request.AmenityId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.AmenitiesPaged}:", cancellationToken);

        return ApiResponse<AmenityDto>.SuccessResponse(
            mapper.Map<AmenityDto>(amenity),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.Amenity), HttpStatusCode.OK);
    }
}