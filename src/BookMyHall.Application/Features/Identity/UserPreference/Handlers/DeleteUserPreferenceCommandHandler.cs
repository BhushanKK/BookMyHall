using MediatR;
using System.Net;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteUserPreferenceCommandHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteUserPreferenceCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserPreferenceCommand request,
        CancellationToken cancellationToken)
    {
        var userPreference = await userPreferenceRepository.GetByIdAsync(request.UserPreferenceId,cancellationToken);

        if (userPreference is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.UserPreference),HttpStatusCode.NotFound);
        }

        await userPreferenceRepository.DeleteAsync(userPreference,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,
                EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}