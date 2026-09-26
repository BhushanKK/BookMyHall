using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorAvailabilityQueryHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IMapper mapper)
    : IRequestHandler<GetVendorAvailabilitiesQuery,ApiResponse<PaginatedResult<VendorAvailabilityDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorAvailabilityDto>>> Handle(GetVendorAvailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
        var result =await vendorAvailabilityRepository.GetAllAsync(request.Request,request.VendorId,cancellationToken);
        var response = new PaginatedResult<VendorAvailabilityDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorAvailabilityDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return ApiResponse<PaginatedResult<VendorAvailabilityDto>>.SuccessResponse(response,
                string.Empty,HttpStatusCode.OK);
    }
}