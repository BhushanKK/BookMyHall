using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record CreateVendorImageCommand(
    Guid VendorId,
    Stream ImageStream,
    string FileName,
    string ContentType,
    long FileSize,
    int DisplayOrder,
    bool IsCoverImage = false,
    Guid? VendorServiceId = null) : IRequest<ApiResponse<Guid>>;
