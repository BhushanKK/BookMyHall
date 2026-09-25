using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorCommand:VendorDto, IRequest<ApiResponse<VendorDto>>;
