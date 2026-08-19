using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreatePermissionCommand :PermissionDto, IRequest<ApiResponse<PermissionDto>>;