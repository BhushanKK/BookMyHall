using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;

public sealed record DeleteHallBlockCommand(Guid HallBlockId) : IRequest<ApiResponse<bool>>;
