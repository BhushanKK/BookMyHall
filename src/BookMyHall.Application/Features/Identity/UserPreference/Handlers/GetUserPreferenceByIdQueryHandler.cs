using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetUserPreferenceByIdQueryHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetUserPreferenceByIdQuery, ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(GetUserPreferenceByIdQuery request,CancellationToken cancellationToken)
    {
        var userPreference = await userPreferenceRepository.GetByUserIdAsync(request.UserId,cancellationToken);

        if (userPreference is null)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.UserPreference),HttpStatusCode.NotFound);
        }

        return ApiResponse<UserPreferenceDto>.SuccessResponse(mapper.Map<UserPreferenceDto>(userPreference),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}