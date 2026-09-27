using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorSubCategoryCommand():VendorSubCategoryDto, IRequest<ApiResponse<VendorSubCategoryDto>>;