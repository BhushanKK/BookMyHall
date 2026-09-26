using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorCommand: VendorDto, IRequest<ApiResponse<VendorDto>>;