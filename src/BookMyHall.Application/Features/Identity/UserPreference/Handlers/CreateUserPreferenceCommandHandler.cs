using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateUserPreferenceCommandHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateUserPreferenceCommand> validator,
    IMessageHelper messageHelper,
    ICurrentUser currentUser)
    : IRequestHandler<CreateUserPreferenceCommand, ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(
        CreateUserPreferenceCommand request,
        CancellationToken cancellationToken)
    {

        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                "User authentication is required.",
                HttpStatusCode.Unauthorized);
        }

        request.UserId = currentUser.UserId.Value;

        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            return ApiResponse<UserPreferenceDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }


        var userPreference = mapper.Map<UserPreference>(request);

        try
        {
            await userPreferenceRepository.AddAsync(
                userPreference,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.UserPreference),
                HttpStatusCode.Conflict);
        }

       
        return ApiResponse<UserPreferenceDto>.SuccessResponse(
            mapper.Map<UserPreferenceDto>(userPreference),
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.UserPreference),
            HttpStatusCode.Created);
    }
}