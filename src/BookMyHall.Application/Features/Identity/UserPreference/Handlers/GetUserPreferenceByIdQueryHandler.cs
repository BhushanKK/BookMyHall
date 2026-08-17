using MediatR;
using System.Net;
using AutoMapper;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetUserPreferenceByIdQueryHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetUserPreferenceByIdQuery, ApiResponse<UserPreference>>
{
    public async Task<ApiResponse<UserPreference>> Handle(GetUserPreferenceByIdQuery request,CancellationToken cancellationToken)
    {
        var userPreference = await userPreferenceRepository.GetByIdAsync(request.UserPreferenceId,cancellationToken);

        if (userPreference is null)
        {
            return ApiResponse<UserPreference>.FailureResponse(messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.UserPreference),HttpStatusCode.NotFound);
        }

        return ApiResponse<UserPreference>.SuccessResponse(mapper.Map<UserPreference>(userPreference),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}