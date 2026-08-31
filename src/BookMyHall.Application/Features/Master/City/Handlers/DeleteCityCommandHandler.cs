using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteCityCommandHandler(
    ICityRepository cityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteCityCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.City), HttpStatusCode.NotFound);
        }

        city.IsDeleted = true;
        await cityRepository.UpdateAsync(city, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Cities}:{request.CityId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CitiesPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.City), HttpStatusCode.OK);
    }
}