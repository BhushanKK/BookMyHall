using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorServiceCommandHandler(IVendorServiceRepository vendorServiceRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteVendorServiceCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorServiceCommand request,CancellationToken cancellationToken)
    {
        var vendorService =await vendorServiceRepository.GetByIdAsync(request.VendorServiceId,cancellationToken);

        if (vendorService is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorService),
                HttpStatusCode.NotFound);
        }

        vendorService.IsDeleted = true;
        vendorService.IsActive = false;

        await vendorServiceRepository.UpdateAsync(vendorService,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.VendorServices}:{vendorService.VendorServiceId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServicesPaged}:",cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorService.VendorId}",cancellationToken);

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.DeletedEntity(
                ResourceNames.Entities,
                EntityKeys.VendorService),
            HttpStatusCode.OK);
    }
}