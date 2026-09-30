using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class DeleteVendorServiceAreaCommandHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteVendorServiceAreaCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var vendorServiceArea =await vendorServiceAreaRepository.GetByIdAsync(request.VendorServiceAreaId,
                cancellationToken);
        if (vendorServiceArea is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFoundEntity(
                        ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.NotFound);
        }

        if (vendorServiceArea.IsDeleted)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFoundEntity(
                        ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.NotFound);
        }

        vendorServiceArea.IsDeleted = true;
        vendorServiceArea.IsActive = false;

        try
        {
            await vendorServiceAreaRepository.UpdateAsync(vendorServiceArea,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.AlreadyExistsEntity(
                        ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorServiceArea.VendorServiceAreaId,cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(ResourceNames.Entities,
        EntityKeys.VendorServiceArea),HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorServiceAreaId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorServiceAreas}:{vendorServiceAreaId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServiceAreasPaged}:",cancellationToken);
    }
}