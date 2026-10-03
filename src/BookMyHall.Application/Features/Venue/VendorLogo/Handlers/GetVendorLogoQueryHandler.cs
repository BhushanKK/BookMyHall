using System.Net;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorLogoQueryHandler(IVendorRepository repository, IR2StorageService storage)
    : IRequestHandler<GetVendorLogoQuery, ApiResponse<VendorLogoDto>>
{
    public async Task<ApiResponse<VendorLogoDto>> Handle(GetVendorLogoQuery request, CancellationToken cancellationToken)
    {
        var vendor = await repository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null || string.IsNullOrWhiteSpace(vendor.LogoUrl))
        {
            return ApiResponse<VendorLogoDto>.FailureResponse("Vendor logo not found.", HttpStatusCode.NotFound);
        }

        var url = await storage.GetPreSignedUrlAsync(vendor.LogoUrl, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorLogoDto>.SuccessResponse(new(vendor.VendorId, url), "Vendor logo retrieved successfully.", HttpStatusCode.OK);
    }
}
