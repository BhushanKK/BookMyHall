using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record UpdateHallImageCommand(
    Guid HallImageId,
    bool IsCoverImage,
    int DisplayOrder,
    bool IsActive,
    Stream? ImageStream,
    string? FileName,
    string? ContentType,
    long? FileSize)
    : IRequest<ApiResponse<HallImageDto>>;