using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorOwnersAutoCompleteQueryHandler(IVendorRepository repository)
    : IRequestHandler<GetVendorOwnersAutoCompleteQuery, ApiResponse<IReadOnlyList<AutoCompleteItem>>>
{
    public async Task<ApiResponse<IReadOnlyList<AutoCompleteItem>>> Handle(
        GetVendorOwnersAutoCompleteQuery request, CancellationToken cancellationToken)
    {
        var owners = await repository.GetOwnerAutoCompleteAsync(
            request.SearchTerm?.Trim(), Math.Clamp(request.Limit, 1, 20), cancellationToken);
        return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse(owners);
    }
}
