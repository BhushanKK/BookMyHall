using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateUserPreferenceCommandHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<UpdateUserPreferenceCommand> validator,
    IMessageHelper messageHelper,ICurrentUser currentUser)
    : IRequestHandler<UpdateUserPreferenceCommand,ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(
        UpdateUserPreferenceCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                "User authentication is required.",
                HttpStatusCode.Unauthorized);
        }

        request.UserId = currentUser.UserId.Value;
        var userId = request.UserId;

        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(error => error.ErrorMessage));
            return ApiResponse<UserPreferenceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var userPreference =await userPreferenceRepository.GetByUserIdAsync(userId,cancellationToken);
        if (userPreference is null)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.UserPreference),HttpStatusCode.NotFound);
        }

        mapper.Map(request, userPreference);
        await userPreferenceRepository.UpdateAsync(userPreference,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<UserPreferenceDto>(userPreference);
        return ApiResponse<UserPreferenceDto>.SuccessResponse(response,
            messageHelper.UpdatedEntity(ResourceNames.Entities,
                EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}