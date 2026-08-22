using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateHallImageCommand, ApiResponse<HallImageDto>>
{
    public async Task<ApiResponse<HallImageDto>> Handle(
        UpdateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        var hallImage = await hallImageRepository.GetByIdAsync(request.HallImageId, cancellationToken);

        if (hallImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        hallImage.IsCoverImage = request.IsCoverImage;
        hallImage.DisplayOrder = request.DisplayOrder;
        hallImage.IsActive = request.IsActive;

        await hallImageRepository.UpdateAsync(hallImage, cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.HallImage}:{request.HallImageId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallImage}:", cancellationToken);
        
        return ApiResponse<HallImageDto>.SuccessResponse
        (
            mapper.Map<HallImageDto>(hallImage),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}