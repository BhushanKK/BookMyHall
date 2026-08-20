using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateMenuCommand
    : MenuDto, IRequest<ApiResponse<MenuDto>>;