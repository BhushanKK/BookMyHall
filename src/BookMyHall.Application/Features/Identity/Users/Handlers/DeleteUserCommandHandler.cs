using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        user.Deactivate();
        user.IsDeleted = true;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var cacheKey = $"{CacheKeys.Users}:{request.UserId}";
        await cacheService.RemoveAsync(cacheKey, cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}