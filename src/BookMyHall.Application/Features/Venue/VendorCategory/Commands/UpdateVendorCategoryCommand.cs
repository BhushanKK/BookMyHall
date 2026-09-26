using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorCategoryCommand: VendorCategoryDto, IRequest<ApiResponse<VendorCategoryDto>>;