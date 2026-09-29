using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorPackageCommandHandler(IVendorPackageRepository repository,
    IUnitOfWork unitOfWork, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteVendorPackageCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVendorPackageCommand request, CancellationToken cancellationToken)
    {
        var package = await repository.GetByIdAsync(request.VendorPackageId, cancellationToken);
        if (package is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.VendorPackage), HttpStatusCode.NotFound);
        }

        package.IsDeleted = true;
        package.IsActive = false;
        await repository.UpdateAsync(package, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.VendorPackages}:{request.VendorPackageId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorPackagesPaged}:", cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.OK);
    }
}
