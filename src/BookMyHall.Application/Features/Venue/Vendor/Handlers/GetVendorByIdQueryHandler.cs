using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorByIdQueryHandler(
    IVendorRepository vendorRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetVendorByIdQuery,
        ApiResponse<VendorDto>>
{
    public async Task<ApiResponse<VendorDto>> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            $"{CacheKeys.Vendors}:{request.VendorId}";

        var cachedVendor =
            await cacheService.GetAsync<VendorDto>(
                cacheKey,
                cancellationToken);

        if (cachedVendor is not null)
        {
            return ApiResponse<VendorDto>.SuccessResponse(
                cachedVendor,
                string.Empty,
                HttpStatusCode.OK);
        }

        var vendor =
            await vendorRepository.GetByIdAsync(
                request.VendorId,
                cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorDto>.FailureResponse(
                messageHelper.NotFound(
                    EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        var response =mapper.Map<VendorDto>(vendor);

        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<VendorDto>.SuccessResponse(
            response,
            string.Empty,
            HttpStatusCode.OK);
    }
}