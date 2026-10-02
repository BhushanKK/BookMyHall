using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorBusinessesAutoCompleteQuery(
    Guid VendorId,
    string? SearchTerm,
    Guid? AreaId = null,
    int Limit = 20)
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;