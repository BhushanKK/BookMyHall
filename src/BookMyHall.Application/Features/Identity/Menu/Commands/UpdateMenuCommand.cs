using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuCommand()
    :MenuDto,IRequest<ApiResponse<MenuDto>>;