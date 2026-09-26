using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorsQueryHandler(
    IVendorRepository vendorRepository,
    IMapper mapper)
    : IRequestHandler<
        GetVendorsQuery,
        ApiResponse<PaginatedResult<VendorDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorDto>>> Handle(
        GetVendorsQuery request,
        CancellationToken cancellationToken)
    {
        var result =
            await vendorRepository.GetAllAsync(
                request.Request,
                cancellationToken);

        var response = new PaginatedResult<VendorDto>
        {
            Items = mapper.Map<
                IReadOnlyList<VendorDto>>(
                    result.Items),

            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return ApiResponse<
            PaginatedResult<VendorDto>>.SuccessResponse(
                response,
                string.Empty,
                HttpStatusCode.OK);
    }
}