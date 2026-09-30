using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorPackageItemCommandHandler(IVendorPackageItemRepository vendorPackageItemRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteVendorPackageItemCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorPackageItemCommand request,CancellationToken cancellationToken)
    {
        var vendorpackageitem = await vendorPackageItemRepository.GetByIdAsync(request.VendorPackageItemId, cancellationToken);

        if (vendorpackageitem is null)
        {
            return ApiResponse<bool>.FailureResponse( messageHelper.NotFound(EntityKeys.VendorPackageItem), HttpStatusCode.NotFound);
        }

        vendorpackageitem.IsDeleted = true;
        await vendorPackageItemRepository.UpdateAsync(vendorpackageitem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var cacheKey=$"{CacheKeys.VendorPackageItems}:{request.VendorPackageItemId}";
        await cacheService.RemoveAsync(cacheKey, cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorPackageItemsPaged}:", cancellationToken);
      
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.OK);
    }
}