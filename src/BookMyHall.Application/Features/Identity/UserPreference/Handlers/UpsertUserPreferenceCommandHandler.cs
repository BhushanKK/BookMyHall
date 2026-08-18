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

public sealed class UpsertUserPreferenceCommandHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpsertUserPreferenceCommand> validator,
    IMessageHelper messageHelper,
    ICurrentUser currentUser)
    : IRequestHandler<UpsertUserPreferenceCommand,
        ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(
        UpsertUserPreferenceCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Authentication
        // ---------------------------------------------------------
        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse
            (
                "User authentication is required.",
                HttpStatusCode.Unauthorized
            );
        }

        var userId = currentUser.UserId.Value;
        // Always use authenticated user.
        request.UserId = userId;

        // ---------------------------------------------------------
        // 2. Validation
        // ---------------------------------------------------------
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(error => error.ErrorMessage));
            return ApiResponse<UserPreferenceDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        // ---------------------------------------------------------
        // 3. Check existing preference
        // ---------------------------------------------------------
        var existingPreference = await userPreferenceRepository.GetByUserIdAsync(userId, cancellationToken);

        try
        {
            // =====================================================
            // UPDATE
            // =====================================================
            if (existingPreference is not null)
            {
                mapper.Map(request, existingPreference);
                await userPreferenceRepository.UpdateAsync(existingPreference, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<UserPreferenceDto>.SuccessResponse
                (
                    mapper.Map<UserPreferenceDto>(existingPreference),
                    messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.UserPreference),
                    HttpStatusCode.OK
                );
            }

            // =====================================================
            // CREATE
            // =====================================================
            var userPreference = mapper.Map<UserPreference>(request);

            userPreference.UserId = userId;
            await userPreferenceRepository.AddAsync(userPreference, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<UserPreferenceDto>.SuccessResponse
            (
                mapper.Map<UserPreferenceDto>(userPreference),
                messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.UserPreference),
                HttpStatusCode.Created  
            );
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.UserPreference),
                HttpStatusCode.Conflict
            );
        }
    }
}