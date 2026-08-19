using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record DeletePermissionCommand(Guid PermissionId)
    : IRequest<ApiResponse<bool>>;