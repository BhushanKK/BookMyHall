using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateUserPreferenceCommandHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper): IRequestHandler<UpdateUserPreferenceCommand,ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(UpdateUserPreferenceCommand request,CancellationToken cancellationToken)
    {
        var userPreference =await userPreferenceRepository.GetByUserIdAsync(request.UserId,cancellationToken);

        if (userPreference is null)
        {
            userPreference = UserPreference.Create(request.UserId);
            mapper.Map(request, userPreference);
            await userPreferenceRepository.AddAsync(userPreference,cancellationToken);
        }
        else
        {
            mapper.Map(request, userPreference);
            await userPreferenceRepository.UpdateAsync(userPreference,cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<UserPreferenceDto>(userPreference);

        return ApiResponse<UserPreferenceDto>.SuccessResponse(response,
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}