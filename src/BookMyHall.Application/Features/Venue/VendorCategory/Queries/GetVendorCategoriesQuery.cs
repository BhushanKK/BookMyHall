using BookMyHall.Contracts.Common;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorCategoriesQuery(PaginationRequest Request): IRequest<ApiResponse<PaginatedResult<VendorCategoryDto>>>;