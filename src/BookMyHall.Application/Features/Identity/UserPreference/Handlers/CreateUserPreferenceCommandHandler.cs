// using MediatR;
// using System.Net;
// using AutoMapper;
// using FluentValidation;
// using BookMyHall.Application.Abstractions.Persistence;
// using BookMyHall.Application.Abstractions.Persistence.Repositories;
// using BookMyHall.Application.Abstractions.Security;
// using BookMyHall.Contracts.Common;
// using BookMyHall.Domain.Entities.Identity;
// using BookMyHall.Persistence.Exceptions;
// using BookMyHall.Shared.Common;
// using BookMyHall.Shared.Constants;

// namespace BookMyHall.Application.Features.Identity;

// public sealed class CreateUserPreferenceCommandHandler(
//     IUserPreferenceRepository userPreferenceRepository,
//     IUnitOfWork unitOfWork,IMapper mapper,
//     IValidator<CreateUserPreferenceCommand> validator,
//     IMessageHelper messageHelper,ICurrentUser currentUser)
//     : IRequestHandler<CreateUserPreferenceCommand,ApiResponse<UserPreferenceDto>>
// {
//     public async Task<ApiResponse<UserPreferenceDto>> Handle( CreateUserPreferenceCommand request,
//         CancellationToken cancellationToken)
//     {
//         var userId = currentUser.UserId;
//         if (!userId.HasValue)
//         {
//             return ApiResponse<UserPreferenceDto>.FailureResponse("User authentication is required.",
//                 HttpStatusCode.Unauthorized);
//         }

//         request.UserId = userId.Value;
//         var validationResult = await validator.ValidateAsync(request,cancellationToken);
//         if (!validationResult.IsValid)
//         {
//             var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
//             return ApiResponse<UserPreferenceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
//         }
//         var userPreference = mapper.Map<UserPreference>(request);

//         try
//         {
//             await userPreferenceRepository.AddAsync(userPreference,cancellationToken);
//             await unitOfWork.SaveChangesAsync(cancellationToken);
//         }
//         catch (DuplicateRecordException)
//         {
//             return ApiResponse<UserPreferenceDto>.FailureResponse(
//                 messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
//                     EntityKeys.UserPreference), HttpStatusCode.Conflict);
//         }

//         return ApiResponse<UserPreferenceDto>.SuccessResponse(
//             mapper.Map<UserPreferenceDto>(userPreference),
//             messageHelper.AddedEntity(ResourceNames.Entities,
//                 EntityKeys.UserPreference),HttpStatusCode.Created);
//     }
// }

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
    : IRequestHandler<
        CreateUserPreferenceCommand,
        ApiResponse<UserPreferenceDto>>
{
    public async Task<ApiResponse<UserPreferenceDto>> Handle(
        CreateUserPreferenceCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate authenticated user
        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<UserPreferenceDto>.FailureResponse(
                "User authentication is required.",
                HttpStatusCode.Unauthorized);
        }

        // 2. Set user from authenticated context
        request.UserId = currentUser.UserId.Value;

        // 3. Validate request
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

        // 4. Map request to domain entity
        var userPreference = mapper.Map<UserPreference>(request);

        // 5. Persist entity
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

        // 6. Return created response
        return ApiResponse<UserPreferenceDto>.SuccessResponse(
            mapper.Map<UserPreferenceDto>(userPreference),
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.UserPreference),
            HttpStatusCode.Created);
    }
}