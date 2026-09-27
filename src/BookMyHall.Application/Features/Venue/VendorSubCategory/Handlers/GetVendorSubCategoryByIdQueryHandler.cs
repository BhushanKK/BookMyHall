using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorSubCategoryByIdQueryHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetVendorSubCategoryByIdQuery,ApiResponse<VendorSubCategoryDto>>
{
    public async Task<ApiResponse<VendorSubCategoryDto>> Handle(GetVendorSubCategoryByIdQuery request,CancellationToken cancellationToken)
    {
        var vendorSubCategory =await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId,cancellationToken);
        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorSubCategory),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<VendorSubCategoryDto>.SuccessResponse(mapper.Map<VendorSubCategoryDto>(vendorSubCategory),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.OK);
    }
}