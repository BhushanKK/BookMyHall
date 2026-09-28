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
        ApiResponse<PaginatedResponse<VendorServiceDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorServiceDto>>> Handle(GetVendorServicesQuery request,
        CancellationToken cancellationToken)
    {
        var result =await vendorServiceRepository.GetAllAsync(request.Request,
        request.VendorId,request.VendorSubCategoryId,cancellationToken);
        var response = new PaginatedResponse<VendorServiceDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorServiceDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
      return ApiResponse<PaginatedResponse<VendorServiceDto>>.SuccessResponse(response,string.Empty,HttpStatusCode.OK);
    }
}