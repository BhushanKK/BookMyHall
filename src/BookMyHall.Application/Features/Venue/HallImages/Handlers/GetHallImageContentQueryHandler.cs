using MediatR;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Shared.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImageContentQueryHandler(
    IHallImageRepository hallImageRepository,
    IR2StorageService r2StorageService)
    : IRequestHandler<GetHallImageContentQuery, HallImageContentResult?>
{
    public async Task<HallImageContentResult?> Handle(
        GetHallImageContentQuery request,
        CancellationToken cancellationToken)
    {
        var hallImage = await hallImageRepository.GetByIdAsync(request.HallImageId, cancellationToken);

        if (hallImage is null || !hallImage.IsActive)
            return null;

        var stream = await r2StorageService.GetAsync(hallImage.ImageUrl, cancellationToken);

        if (stream is null)
            return null;

        var contentType =FileContents.GetContentType(hallImage.ImageUrl);
        return new HallImageContentResult(stream, contentType);
    }
}