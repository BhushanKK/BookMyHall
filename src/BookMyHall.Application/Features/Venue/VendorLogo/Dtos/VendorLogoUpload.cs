namespace BookMyHall.Application.Features.Venue;

public sealed record VendorLogoUpload(Stream Stream, string FileName, string ContentType, long FileSize);
