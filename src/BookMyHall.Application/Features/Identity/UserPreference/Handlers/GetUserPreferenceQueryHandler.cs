using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetUserPreferenceQueryHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IMessageHelper messageHelper,
    IMapper mapper): IRequestHandler<GetUserPreferenceQuery,ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(GetUserPreferenceQuery request,CancellationToken cancellationToken)
    {
        var userPreference =await userPreferenceRepository.GetByUserIdAsync(request.UserId,cancellationToken);

        if (userPreference is null)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.UserPreference),
                HttpStatusCode.NotFound);
        }

        var response = mapper.Map<UserPreferenceDto>(userPreference);

        return ApiResponse<UserPreferenceDto>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}