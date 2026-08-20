using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserProfileImageCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IR2StorageService r2StorageService,
    IMapper mapper,
    IValidator<UpdateUserProfileImageCommand> validator,
    IMessageHelper messageHelper,
    IConfiguration configuration)
    : IRequestHandler<
        UpdateUserProfileImageCommand,
        ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(UpdateUserProfileImageCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<UserDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var user = await userRepository.GetByIdAsync(request.UserId,cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.User),HttpStatusCode.NotFound);
        }

        try
        {
            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

            var objectKey = $"users/{request.UserId}/profile-picture{extension}";

            await r2StorageService.UploadAsync(request.ImageStream,objectKey,request.ContentType,cancellationToken);

            var publicBaseUrl = configuration["CloudflareR2:PublicBaseUrl"];

            if (string.IsNullOrWhiteSpace(publicBaseUrl))
            {
                throw new InvalidOperationException("Cloudflare R2 public base URL is not configured.");
            }

            var imageUrl =$"{publicBaseUrl.TrimEnd('/')}/{objectKey}";

            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            {
                var oldObjectKey = ExtractObjectKey(user.ProfileImageUrl,publicBaseUrl);

                if (!string.IsNullOrWhiteSpace(oldObjectKey) &&
                    !string.Equals(oldObjectKey,objectKey,StringComparison.OrdinalIgnoreCase))
                {
                    await r2StorageService.DeleteAsync(oldObjectKey,cancellationToken);
                }
            }

            user.ProfileImageUrl = imageUrl;
            await userRepository.UpdateAsync(user,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.User),HttpStatusCode.Conflict);
        }

        // 10. Return response
        return ApiResponse<UserDto>.SuccessResponse(mapper.Map<UserDto>(user),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.User),HttpStatusCode.OK);
    }

    private static string? ExtractObjectKey(string imageUrl,string publicBaseUrl)
    {
        var baseUrl = publicBaseUrl.TrimEnd('/');

        if (!imageUrl.StartsWith(baseUrl,StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return imageUrl[baseUrl.Length..].TrimStart('/');
    }
}