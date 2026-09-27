using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorServicesQueryHandler(
    IVendorServiceRepository vendorServiceRepository,
    IMapper mapper)
    : IRequestHandler<
        GetVendorServicesQuery,
        ApiResponse<PaginatedResult<VendorServiceDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorServiceDto>>> Handle(GetVendorServicesQuery request,
        CancellationToken cancellationToken)
    {
        var result =await vendorServiceRepository.GetAllAsync(request.Request,
        request.VendorId,request.VendorSubCategoryId,cancellationToken);
        var response = new PaginatedResult<VendorServiceDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorServiceDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
      return ApiResponse<PaginatedResult<VendorServiceDto>>.SuccessResponse(response,string.Empty,HttpStatusCode.OK);
    }
}