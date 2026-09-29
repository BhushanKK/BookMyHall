using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorPackageByIdQueryHandler(IVendorPackageRepository repository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorPackageByIdQuery, ApiResponse<VendorPackage>>
{
    public async Task<ApiResponse<VendorPackage>> Handle(GetVendorPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorPackages}:{request.VendorPackageId}";
        var cachedData = await cacheService.GetAsync<VendorPackage>(cacheKey, cancellationToken);
        
        if (cachedData is not null)
        {
            return ApiResponse<VendorPackage>.SuccessResponse(cachedData, 
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.OK);
        }

        var package = await repository.GetByIdAsync(request.VendorPackageId, cancellationToken);
        if (package is null)
        {
            return ApiResponse<VendorPackage>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.NotFound);
        }
        var response =mapper.Map<VendorPackage>(package);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
       return ApiResponse<VendorPackage>.SuccessResponse
        (response,messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackage),
            HttpStatusCode.OK
        );
    }
}
