using System.Net;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteVendorImageCommandHandler(
    IVendorImageRepository repository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteVendorImageCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteVendorImageCommand request,
        CancellationToken cancellationToken)
    {
        var image = await repository.GetByIdAsync(request.VendorImageId, cancellationToken);
        if (image is null || !image.IsActive)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.VendorImage),
                HttpStatusCode.NotFound);
        }

        image.UpdateMetadata(image.DisplayOrder, false, false);
        await repository.UpdateAsync(image, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await VendorImageCacheInvalidator.InvalidateAsync(
            cacheService,
            image.VendorId,
            image.VendorImageId,
            cancellationToken);

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.VendorImage),
            HttpStatusCode.OK);
    }
}