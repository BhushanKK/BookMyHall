using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteAmenityCommandHandler(
    IAmenityRepository amenityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteAmenityCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteAmenityCommand request,
        CancellationToken cancellationToken)
    {
        var amenity = await amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);

        if (amenity is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Amenity),
                HttpStatusCode.NotFound);
        }

        amenity.IsDeleted = true;
        await amenityRepository.UpdateAsync(amenity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Amenity}:{request.AmenityId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.AmenitiesPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.Amenity), HttpStatusCode.OK);
    }
}