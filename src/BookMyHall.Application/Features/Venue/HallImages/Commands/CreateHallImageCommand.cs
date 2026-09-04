using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record CreateHallImageCommand(
    Guid HallId,
    Stream ImageStream,
    string FileName,
    string ContentType,
    long FileSize,
    int DisplayOrder,
    bool IsCoverImage=false)
    : IRequest<ApiResponse<Guid>>;