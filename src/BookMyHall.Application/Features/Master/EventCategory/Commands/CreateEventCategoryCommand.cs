using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateEventCategoryCommand
    :EventCategoryDto, IRequest<ApiResponse<EventCategoryDto>>;