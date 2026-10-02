using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Shared.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorImageContentQueryHandler(
    IVendorImageRepository repository,
    IR2StorageService storage)
    : IRequestHandler<GetVendorImageContentQuery, VendorImageContentResult?>
{
    public async Task<VendorImageContentResult?> Handle(
        GetVendorImageContentQuery request,
        CancellationToken cancellationToken)
    {
        var image = await repository.GetByIdAsync(request.VendorImageId, cancellationToken);
        if (image is null || !image.IsActive)
        {
            return null;
        }

        var stream = await storage.GetAsync(image.ImageUrl, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        return new VendorImageContentResult(stream, FileContents.GetContentType(image.ImageUrl));
    }
}