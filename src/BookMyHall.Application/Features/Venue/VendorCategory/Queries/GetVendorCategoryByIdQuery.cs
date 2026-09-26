using BookMyHall.Contracts.Common;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorCategoryByIdQuery(Guid VendorCategoryId): IRequest<ApiResponse<VendorCategoryDto>>;