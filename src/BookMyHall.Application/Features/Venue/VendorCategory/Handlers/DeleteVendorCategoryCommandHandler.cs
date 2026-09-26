using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorCategoryCommandHandler(IVendorCategoryRepository vendorCategoryRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteVendorCategoryCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorCategoryCommand request,CancellationToken cancellationToken)
    {
        var vendorcategory =await vendorCategoryRepository.GetByIdAsync(request.VendorCategoryId,cancellationToken);

        if (vendorcategory is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(
                    EntityKeys.VendorCategory), HttpStatusCode.NotFound);
        }

        vendorcategory.IsDeleted = true;
        vendorcategory.IsActive = false;

        await vendorCategoryRepository.UpdateAsync(vendorcategory,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.VendorCategories}:{vendorcategory.VendorCategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorCategoriesPaged}:",cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.OK);
    }
}