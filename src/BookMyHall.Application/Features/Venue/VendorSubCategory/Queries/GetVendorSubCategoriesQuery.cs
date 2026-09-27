using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorSubCategoriesQuery(PaginationRequest Request,Guid? VendorCategoryId)
 :IRequest<ApiResponse<PaginatedResult<VendorSubCategoryDto>>>;
