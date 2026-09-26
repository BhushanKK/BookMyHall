using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorAvailabilityCommandHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteVendorAvailabilityCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorAvailabilityCommand request,CancellationToken cancellationToken)
    {
        var vendoravailability =await vendorAvailabilityRepository.GetByIdAsync(request.VendorAvailabilityId,cancellationToken);
        if (vendoravailability is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(
                    EntityKeys.VendorAvailability),HttpStatusCode.NotFound);
        }

        vendoravailability.IsDeleted = true;
        vendoravailability.IsActive = false;
        await vendorAvailabilityRepository.UpdateAsync(vendoravailability,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.VendorAvailabilities}:{vendoravailability.VendorAvailabilityId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorAvailabilitiesPaged}:",cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.VendorAvailability),HttpStatusCode.OK);
    }
}