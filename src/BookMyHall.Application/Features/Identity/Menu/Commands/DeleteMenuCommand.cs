using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record DeleteMenuCommand (Guid MenuId)
    : IRequest<ApiResponse<bool>>;