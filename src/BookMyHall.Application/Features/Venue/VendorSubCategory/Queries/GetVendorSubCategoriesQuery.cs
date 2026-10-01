using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorSubCategoriesQuery(PaginationRequest Request,Guid? VendorCategoryId)
 :IRequest<ApiResponse<PaginatedResponse<VendorSubCategory>>>;
