using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class DeleteVendorSubCategoryCommandHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteVendorSubCategoryCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorSubCategoryCommand request,CancellationToken cancellationToken)
    {
        var vendorSubCategory = await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId, cancellationToken);
        if (vendorSubCategory is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorSubCategory),
                HttpStatusCode.NotFound);
        }

        vendorSubCategory.IsDeleted = true;
        vendorSubCategory.IsActive = false;
        await vendorSubCategoryRepository.UpdateAsync(vendorSubCategory,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.VendorSubCategories}:{vendorSubCategory.VendorSubCategoryId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorSubCategoriesPaged}:",cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.OK);
    }
}