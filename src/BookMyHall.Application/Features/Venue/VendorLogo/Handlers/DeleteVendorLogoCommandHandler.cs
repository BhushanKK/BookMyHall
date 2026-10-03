using System.Net;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorLogoCommandHandler(
    IVendorRepository repository,
    IUnitOfWork unitOfWork,
    IR2StorageService storage,
    ICacheService cache,
    ICurrentUser currentUser,
    ILogger<DeleteVendorLogoCommandHandler> logger)
    : IRequestHandler<DeleteVendorLogoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorLogoCommand request, CancellationToken cancellationToken)
    {
        var vendor = await repository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<bool>.FailureResponse("Vendor not found.", HttpStatusCode.NotFound);
        }

        if (!currentUser.Roles.Contains(RoleConstants.Admin) && !currentUser.Roles.Contains(RoleConstants.HallOwner) &&
            (!currentUser.UserId.HasValue || vendor.UserId != currentUser.UserId))
        {
            return ApiResponse<bool>.FailureResponse("You cannot update this vendor's logo.", HttpStatusCode.Forbidden);
        }

        var previousKey = vendor.LogoUrl;
        vendor.LogoUrl = null;
        await repository.UpdateAsync(vendor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await VendorLogoStorage.DeleteSafelyAsync(storage, previousKey, logger);
        await VendorLogoStorage.InvalidateCacheAsync(cache, vendor.VendorId, cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "Vendor logo removed successfully.", HttpStatusCode.OK);
    }
}
