using System.Net;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallBlockCommandHandler(IHallBlockRepository hallBlockRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteHallBlockCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteHallBlockCommand request,CancellationToken cancellationToken)
    {
        var hallBlock = await hallBlockRepository.GetByIdAsync(request.HallBlockId,cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFoundEntity(
                ResourceNames.Entities,EntityKeys.HallBlock),HttpStatusCode.NotFound);
        }

        hallBlock.IsActive = false;
        await hallBlockRepository.UpdateAsync(hallBlock,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.HallBlock}:{request.HallBlockId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallBlock}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,
                EntityKeys.HallBlock),HttpStatusCode.OK);
    }
}