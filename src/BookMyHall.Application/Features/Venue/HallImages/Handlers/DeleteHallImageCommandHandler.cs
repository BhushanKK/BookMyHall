using MediatR;
using System.Net;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteHallImageCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteHallImageCommand request,
        CancellationToken cancellationToken)
    {
        var hallImage = await hallImageRepository.GetByIdAsync(request.HallImageId, cancellationToken);

        if (hallImage is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        if (!hallImage.IsActive)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        hallImage.IsDeleted = true;
        hallImage.IsCoverImage = false;

        await hallImageRepository.UpdateAsync(hallImage,cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.HallImage}:{request.HallImageId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallImagesPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}