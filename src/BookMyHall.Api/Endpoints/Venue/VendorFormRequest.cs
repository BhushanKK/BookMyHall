using BookMyHall.Application.Features.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public sealed class VendorFormRequest : VendorRequest
{
    public IFormFile? Logo { get; set; }
}
