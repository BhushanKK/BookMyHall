using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAreaCommand
    : IRequest<ApiResponse<AreaDto>>
{
    public Guid AreaId { get; set; }

    public string AreaName { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public Guid CityId { get; set; }

    public bool IsActive { get; set; }
}