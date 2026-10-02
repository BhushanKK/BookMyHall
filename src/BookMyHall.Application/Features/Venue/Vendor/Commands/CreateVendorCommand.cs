using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorCommand : VendorDto, IRequest<ApiResponse<VendorDto>>;