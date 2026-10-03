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

public sealed class UploadVendorLogoCommandHandler(
    IVendorRepository repository,
    IUnitOfWork unitOfWork,
    IR2StorageService storage,
    ICacheService cache,
    ICurrentUser currentUser,
    ILogger<UploadVendorLogoCommandHandler> logger)
    : IRequestHandler<UploadVendorLogoCommand, ApiResponse<VendorLogoDto>>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    public async Task<ApiResponse<VendorLogoDto>> Handle(UploadVendorLogoCommand request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var expectedContentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null
        };
        if (request.VendorId == Guid.Empty || request.ImageStream is null ||
            request.FileSize is <= 0 or > MaxFileSize || expectedContentType is null ||
            !string.Equals(request.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
        {
            return ApiResponse<VendorLogoDto>.FailureResponse("Provide a valid JPG, PNG, or WEBP logo up to 5 MB.", HttpStatusCode.BadRequest);
        }

        var vendor = await repository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<VendorLogoDto>.FailureResponse("Vendor not found.", HttpStatusCode.NotFound);
        }

        if (!currentUser.Roles.Contains(RoleConstants.Admin) && !currentUser.Roles.Contains(RoleConstants.HallOwner) &&
            (!currentUser.UserId.HasValue || vendor.UserId != currentUser.UserId))
        {
            return ApiResponse<VendorLogoDto>.FailureResponse("You cannot update this vendor's logo.", HttpStatusCode.Forbidden);
        }

        var previousKey = vendor.LogoUrl;
        var key = $"vendors/{vendor.VendorId}/logos/{Guid.NewGuid()}{extension}";
        try
        {
            if (request.ImageStream.CanSeek)
            {
                request.ImageStream.Position = 0;
            }

            await storage.UploadAsync(request.ImageStream, key, expectedContentType, cancellationToken);
            vendor.LogoUrl = key;
            await repository.UpdateAsync(vendor, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            vendor.LogoUrl = previousKey;
            await VendorLogoStorage.DeleteSafelyAsync(storage, key, logger);
            throw;
        }

        await VendorLogoStorage.DeleteSafelyAsync(storage, previousKey, logger);
        await VendorLogoStorage.InvalidateCacheAsync(cache, vendor.VendorId, cancellationToken);
        var url = await storage.GetPreSignedUrlAsync(key, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorLogoDto>.SuccessResponse(new(vendor.VendorId, url), "Vendor logo uploaded successfully.", HttpStatusCode.OK);
    }
}
