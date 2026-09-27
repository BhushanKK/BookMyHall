using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetVendorSubCategoriesQueryHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IMapper mapper)
    : IRequestHandler<GetVendorSubCategoriesQuery,ApiResponse<PaginatedResult<VendorSubCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorSubCategoryDto>>> Handle(GetVendorSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var result =await vendorSubCategoryRepository.GetAllAsync(request.Request,request.VendorCategoryId,cancellationToken);
        var response =new PaginatedResult<VendorSubCategoryDto>
            {
                Items = mapper.Map<IReadOnlyList<VendorSubCategoryDto>>(result.Items),
                PageNumber =result.PageNumber,
                PageSize =result.PageSize,
                TotalCount =result.TotalCount
            };

        return ApiResponse<PaginatedResult<VendorSubCategoryDto>>.SuccessResponse(response,string.Empty,HttpStatusCode.OK);
    }
}