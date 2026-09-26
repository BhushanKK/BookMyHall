using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorCategoriesQueryHandler(IVendorCategoryRepository vendorCategoryRepository,
    IMapper mapper)
    : IRequestHandler<GetVendorCategoriesQuery,ApiResponse<PaginatedResult<VendorCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorCategoryDto>>> Handle(GetVendorCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var result =await vendorCategoryRepository.GetAllAsync(request.Request,cancellationToken);
        var response = new PaginatedResult<VendorCategoryDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorCategoryDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return ApiResponse<PaginatedResult<VendorCategoryDto>>.SuccessResponse(
                response,string.Empty,HttpStatusCode.OK);
    }
}