using MediatR;
using System.Net;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorByIdQueryHandler(
    IVendorRepository vendorRepository,
    IUserRepository userRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<GetVendorByIdQuery, ApiResponse<VendorDto>>
{
    public async Task<ApiResponse<VendorDto>> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.VendorId == Guid.Empty)
        {
            return ApiResponse<VendorDto>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound
            );
        }

        var cacheKey = $"{CacheKeys.Vendors}:{request.VendorId}";

        var cachedVendor = await cacheService.GetAsync<VendorDto>(cacheKey, cancellationToken);

        if (cachedVendor is not null)
        {
            return ApiResponse<VendorDto>.SuccessResponse
            (
                cachedVendor,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.OK
            );
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorDto>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound
            );
        }

        var user = vendor.UserId.HasValue
            ? await userRepository.GetByIdAsync(vendor.UserId.Value, cancellationToken)
            : null;

        var response = new VendorDto
        {
            VendorId = vendor.VendorId,
            UserId = vendor.UserId,
            VendorName = user?.FullName ?? vendor.BusinessName,
            BusinessName = vendor.BusinessName,
            DisplayName = vendor.DisplayName,
            Description = vendor.Description,
            ContactPersonName = vendor.ContactPersonName,
            Email = vendor.Email,
            MobileNumber = vendor.MobileNumber,
            AlternateMobileNumber = vendor.AlternateMobileNumber,
            WebsiteUrl = vendor.WebsiteUrl,
            YoutubeUrl = vendor.YoutubeUrl,
            InstagramUrl = vendor.InstagramUrl,
            AddressLine1 = vendor.AddressLine1,
            AddressLine2 = vendor.AddressLine2,
            Pincode = vendor.Pincode,
            Latitude = vendor.Latitude,
            Longitude = vendor.Longitude,
            EstablishedYear = vendor.EstablishedYear,
            IsVerified = vendor.IsVerified,
            IsActive = vendor.IsActive,
            IsDeleted = vendor.IsDeleted,
            Rating = vendor.Rating,
            ReviewCount = vendor.ReviewCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<VendorDto>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK
        );
    }
}