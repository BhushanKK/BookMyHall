using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;
public sealed class CreateVendorCommand:VendorDto, IRequest<ApiResponse<VendorDto>>;
