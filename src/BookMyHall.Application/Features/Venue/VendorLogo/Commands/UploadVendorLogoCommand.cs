using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record UploadVendorLogoCommand(
    Guid VendorId,
    Stream ImageStream,
    string FileName,
    string ContentType,
    long FileSize) : IRequest<ApiResponse<VendorLogoDto>>;
