// using System.Net;
// using AutoMapper;
// using FluentValidation;
// using MediatR;
// using BookMyHall.Application.Abstractions.Persistence;
// using BookMyHall.Application.Abstractions.Persistence.Repositories;
// using BookMyHall.Application.Common.Interfaces.Storage;
// using BookMyHall.Contracts.Common;
// using BookMyHall.Persistence.Exceptions;
// using BookMyHall.Shared.Common;
// using BookMyHall.Shared.Constants;

// namespace BookMyHall.Application.Features.Identity.Users;

// public sealed class UpdateUserCommandHandler(
//     IUserRepository userRepository,
//     IRoleRepository roleRepository,
//     IUnitOfWork unitOfWork,
//     IMapper mapper,
//     IValidator<UpdateUserCommand> validator,
//     IMessageHelper messageHelper,
//     IR2StorageService r2StorageService)
//     : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
// {
//     public async Task<ApiResponse<UserDto>> Handle(
//         UpdateUserCommand request,
//         CancellationToken cancellationToken)
//     {
//         // 1. Validate request
//         var validationResult = await validator.ValidateAsync(
//             request,
//             cancellationToken);

//         if (!validationResult.IsValid)
//         {
//             var message = string.Join( " | ",validationResult.Errors.Select(x => x.ErrorMessage));

//             return ApiResponse<UserDto>.FailureResponse(message,HttpStatusCode.BadRequest);
//         }

//         // 2. Get User
//         var user = await userRepository.GetByIdAsync( request.UserId,cancellationToken);

//         if (user is null)
//         {
//             return ApiResponse<UserDto>.FailureResponse(
//                 messageHelper.NotFoundEntity(
//                     ResourceNames.Entities,
//                     EntityKeys.User),
//                 HttpStatusCode.NotFound);
//         }

//         // 3. Verify Role
//         var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

//         if (role is null)
//         {
//             return ApiResponse<UserDto>.FailureResponse(
//                 messageHelper.NotFoundEntity(
//                     ResourceNames.Entities,
//                     EntityKeys.Role),
//                 HttpStatusCode.NotFound);
//         }

//         try
//         {
//             // 4. Update user basic information
//             user.UpdateUserProfile(
//                 request.FirstName,
//                 request.MiddleName,
//                 request.LastName,
//                 request.MobileNumber,
//                 request.DateOfBirth,
//                 request.Gender,
//                 request.EmailAddress
//                );

//             // 5. Upload profile picture if provided
//             if (request.ImageStream is not null &&
//                 !string.IsNullOrWhiteSpace(request.FileName) &&
//                 !string.IsNullOrWhiteSpace(request.ContentType))
//             {
//                 var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
//                 var objectKey =$"users/{request.UserId}/profile/{Guid.NewGuid()}{extension}";

//                 await r2StorageService.UploadAsync(
//                     request.ImageStream,
//                     objectKey,
//                     request.ContentType,
//                     cancellationToken);

//                 user.UpdateProfilePicture(objectKey);
//             }

//             // 6. Update database
//             await userRepository.UpdateAsync(user,cancellationToken);

//             await unitOfWork.SaveChangesAsync(cancellationToken);
//         }
//         catch (DuplicateRecordException)
//         {
//             return ApiResponse<UserDto>.FailureResponse(
//                 messageHelper.AlreadyExistsEntity(
//                     ResourceNames.Entities,
//                     EntityKeys.User),
//                 HttpStatusCode.Conflict);
//         }

//         // 7. Map response
//         var response = mapper.Map<UserDto>(user);

//         // 8. Return success
//         return ApiResponse<UserDto>.SuccessResponse(
//             response,
//             messageHelper.UpdatedEntity(
//                 ResourceNames.Entities,
//                 EntityKeys.User),
//             HttpStatusCode.OK);
//     }
// }
using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateUserCommand> validator,
    IMessageHelper messageHelper,
    IR2StorageService r2StorageService,
    IConfiguration configuration)
    : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate request
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<UserDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        // 2. Get existing user
        var user = await userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.NotFound);
        }

        // 3. Verify role
        var role = await roleRepository.GetByIdAsync(
            request.RoleId,
            cancellationToken);

        if (role is null)
        {
            return ApiResponse<UserDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.Role),
                HttpStatusCode.NotFound);
        }

        try
        {
            // 4. Update basic user information
            user.UpdateUserProfile(
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.MobileNumber,
                request.DateOfBirth,
                request.Gender,
                request.EmailAddress);

            // 5. Upload profile picture if provided
            if (request.ImageStream is not null &&
                !string.IsNullOrWhiteSpace(request.FileName) &&
                !string.IsNullOrWhiteSpace(request.ContentType))
            {
                var extension = Path.GetExtension(
                    request.FileName)
                    .ToLowerInvariant();

                var objectKey =
                    $"users/{request.UserId}/profile-picture{extension}";

                // Upload new image
                await r2StorageService.UploadAsync(
                    request.ImageStream,
                    objectKey,
                    request.ContentType,
                    cancellationToken);

                // Get public R2 URL
                var publicBaseUrl =
                    configuration["CloudflareR2:PublicBaseUrl"];

                if (string.IsNullOrWhiteSpace(publicBaseUrl))
                {
                    throw new InvalidOperationException(
                        "Cloudflare R2 public base URL is not configured.");
                }

                var imageUrl =
                    $"{publicBaseUrl.TrimEnd('/')}/{objectKey}";

                // Delete old image
                if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                {
                    var oldObjectKey = ExtractObjectKey(
                        user.ProfileImageUrl,
                        publicBaseUrl);

                    if (!string.IsNullOrWhiteSpace(oldObjectKey) &&
                        !string.Equals(
                            oldObjectKey,
                            objectKey,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        await r2StorageService.DeleteAsync(
                            oldObjectKey,
                            cancellationToken);
                    }
                }

                // Save new image URL in User
                user.ProfileImageUrl = imageUrl;
            }

            // 6. Update user entity
            await userRepository.UpdateAsync(
                user,
                cancellationToken);

            // 7. Save everything in database
            await unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.Conflict);
        }

        // 8. Map updated user
        var response = mapper.Map<UserDto>(user);

        // 9. Return success
        return ApiResponse<UserDto>.SuccessResponse(
            response,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.User),
            HttpStatusCode.OK);
    }

    private static string? ExtractObjectKey(
        string imageUrl,
        string publicBaseUrl)
    {
        var baseUrl = publicBaseUrl.TrimEnd('/');

        if (!imageUrl.StartsWith(
                baseUrl,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return imageUrl[baseUrl.Length..]
            .TrimStart('/');
    }
}