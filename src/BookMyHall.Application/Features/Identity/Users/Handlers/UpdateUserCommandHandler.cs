using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateUserCommand> validator,
    IMessageHelper messageHelper,
    IR2StorageService r2StorageService,ICacheService cacheService)
    : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return ApiResponse<UserDto>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );

        mapper.Map(request, user);

        if (request.DateOfBirth.HasValue)
            user.DateOfBirth = request.DateOfBirth.Value.ToUniversalTime();

        if (request.ImageStream is not null &&
            !string.IsNullOrWhiteSpace(request.FileName) &&
            !string.IsNullOrWhiteSpace(request.ContentType))
        {
            var oldObjectKey = user.ProfileImageUrl;
            var newObjectKey = $"Users/{request.UserId}/Profile";

            await r2StorageService.UploadAsync(
                request.ImageStream,
                newObjectKey,
                request.ContentType,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldObjectKey) &&
                !string.Equals(oldObjectKey, newObjectKey, StringComparison.OrdinalIgnoreCase))
                await r2StorageService.DeleteAsync(oldObjectKey, cancellationToken);

            user.ProfileImageUrl = newObjectKey;
        }

        try
        {
            await userRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.Conflict
            );
        }

        var userDto = mapper.Map<UserDto>(user);
        await cacheService.RemoveAsync($"{CacheKeys.Users}:{request.UserId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Users}:", cancellationToken);
        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}