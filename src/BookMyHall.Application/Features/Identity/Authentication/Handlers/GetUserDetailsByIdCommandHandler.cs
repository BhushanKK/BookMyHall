using System.Net;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetUserDetailsByIdQueryHandler(
    IUserRepository userRepository,
    IMessageHelper messageHelper,
    IR2StorageService storageService)
    : IRequestHandler<GetUserDetailsByIdQuery, ApiResponse<UserDetailsView>>
{
    public async Task<ApiResponse<UserDetailsView>> Handle(
        GetUserDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userDetails = await userRepository.GetUserDetailsByIdAsync
        (
            request.userId,
            request.roleId,
            cancellationToken
        );

        if (userDetails is null)
        {
            return ApiResponse<UserDetailsView>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        if (!string.IsNullOrWhiteSpace(userDetails.ProfileImageUrl))
        {
            userDetails.ProfileImageUrl = await storageService.GetPreSignedUrlAsync
            (
                userDetails.ProfileImageUrl,
                TimeSpan.FromDays(6).Add(TimeSpan.FromHours(23)),cancellationToken
            );
        }

        return ApiResponse<UserDetailsView>.SuccessResponse
        (
            userDetails,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}