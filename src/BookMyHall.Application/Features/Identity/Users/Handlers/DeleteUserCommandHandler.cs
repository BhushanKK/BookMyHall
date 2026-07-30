using System.Net;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request,
    CancellationToken cancellationToken)
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

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}