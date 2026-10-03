using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorCommand : VendorRequest, IRequest<ApiResponse<VendorDto>>
{
    public VendorLogoUpload? Logo { get; set; }
}
