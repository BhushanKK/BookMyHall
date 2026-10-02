using System.Net;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorCoverImageQueryHandler(
    IVendorImageRepository repository,
    IR2StorageService storage,
    IMessageHelper messageHelper)
    : IRequestHandler<GetVendorCoverImageQuery, ApiResponse<VendorImageDto>>
{
    public async Task<ApiResponse<VendorImageDto>> Handle(
        GetVendorCoverImageQuery request,
        CancellationToken cancellationToken)
    {
        var image = await repository.GetCoverImageAsync(request.VendorId, cancellationToken);
        if (image is null)
        {
            return ApiResponse<VendorImageDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.VendorImage),
                HttpStatusCode.NotFound);
        }

        var response = await VendorImageDtoFactory.CreateAsync(image, storage, cancellationToken);
        return ApiResponse<VendorImageDto>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorImage),
            HttpStatusCode.OK);
    }
}