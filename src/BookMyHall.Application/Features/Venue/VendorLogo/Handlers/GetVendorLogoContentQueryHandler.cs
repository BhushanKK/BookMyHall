using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Shared.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorLogoContentQueryHandler(IVendorRepository repository, IR2StorageService storage)
    : IRequestHandler<GetVendorLogoContentQuery, VendorLogoContentResult?>
{
    public async Task<VendorLogoContentResult?> Handle(GetVendorLogoContentQuery request, CancellationToken cancellationToken)
    {
        var vendor = await repository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null || string.IsNullOrWhiteSpace(vendor.LogoUrl))
        {
            return null;
        }

        var stream = await storage.GetAsync(vendor.LogoUrl, cancellationToken);
        return stream is null ? null : new VendorLogoContentResult(stream, FileContents.GetContentType(vendor.LogoUrl));
    }
}
