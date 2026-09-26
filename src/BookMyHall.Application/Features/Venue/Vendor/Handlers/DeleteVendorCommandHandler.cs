using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorCommandHandler(
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        DeleteVendorCommand,
        ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteVendorCommand request,
        CancellationToken cancellationToken)
    {
        var vendor =
            await vendorRepository.GetByIdAsync(
                request.VendorId,
                cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(
                    EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        vendor.IsDeleted = true;
        vendor.IsActive = false;

        await vendorRepository.UpdateAsync(
            vendor,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        await cacheService.RemoveAsync(
            $"{CacheKeys.Vendors}:{vendor.VendorId}",
            cancellationToken);

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.VendorsPaged}:",
            cancellationToken);

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.DeletedEntity(
                ResourceNames.Entities,
                EntityKeys.Vendor),
            HttpStatusCode.OK);
    }
}