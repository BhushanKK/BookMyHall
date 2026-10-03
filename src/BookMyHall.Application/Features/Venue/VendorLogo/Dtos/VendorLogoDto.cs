namespace BookMyHall.Application.Features.Venue;

public sealed record VendorLogoDto(Guid VendorId, string? LogoUrl);

public sealed record VendorLogoContentResult(Stream Stream, string ContentType);
