using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteDistrictCommandHandler(
    IDistrictRepository districtRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteDistrictCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDistrictCommand request,CancellationToken cancellationToken)
    {
        var district = await districtRepository.GetByIdAsync(request.DistrictId,cancellationToken);

        if (district is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }

        district.IsDeleted = true;
        await districtRepository.UpdateAsync(district, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Districts}:{request.DistrictId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.DistrictsPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.OK);
    }
}