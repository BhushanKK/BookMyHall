using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record UpdateVendorImageCommand(
    Guid VendorImageId,
    bool IsCoverImage,
    int DisplayOrder,
    bool IsActive,
    Stream? ImageStream,
    string? FileName,
    string? ContentType,
    long? FileSize) : IRequest<ApiResponse<VendorImageDto>>;