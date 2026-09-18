using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository, IRoleRepository roleRepository,
    IUnitOfWork unitOfWork, IMessageHelper messageHelper,
    ICacheService cacheService, IR2StorageService storageService)
    : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Users}:{request.UserId}";
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        var roleIds = request.Roles?
            .Where(roleId => roleId != Guid.Empty)
            .Distinct()
            .ToArray();

        if (roleIds is null || roleIds.Length == 0)
        {
            return ApiResponse<UserDto>.FailureResponse
            ("At least one valid role is required.",HttpStatusCode.BadRequest);
        }

        var roles = await roleRepository.GetByIdsAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Length)
        {
            var existingRoleIds = roles
                .Select(role => role.RoleId)
                .ToHashSet();

            var invalidRoleId = roleIds
                .First(roleId => !existingRoleIds.Contains(roleId));

            return ApiResponse<UserDto>.FailureResponse
            ($"Role with ID '{invalidRoleId}' was not found.",HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            var existingUser = await userRepository.GetByEmailAddressAsync(request.EmailAddress,cancellationToken);

            if (existingUser is not null && existingUser.UserId != user.UserId)
            {
                return ApiResponse<UserDto>.FailureResponse
                (
                    messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.User),
                    HttpStatusCode.Conflict
                );
            }
        }

        user.UpdateUserProfile(
            firstName: request.FirstName,
            middleName: request.MiddleName,
            lastName: request.LastName,
            mobileNumber: request.MobileNumber,
            emailAddress: request.EmailAddress ?? string.Empty);

        if (request.IsActive)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        await userRepository.RemoveUserRolesAsync(user.UserId, cancellationToken);

        var currentDate = DateTimeOffset.UtcNow;

        var userRoles = roles
            .Select(role => new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                CreatedDate = currentDate,
                CreatedBy = user.UpdatedBy
            })
            .ToList();

        await userRepository.AddUserRolesAsync(userRoles, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = await userRepository.GetUserDtoByIdAsync(user.UserId,cancellationToken);

        if (userDto is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        if (!string.IsNullOrWhiteSpace(userDto.ProfileImageUrl))
        {
            userDto.ProfileImageUrl = await storageService.GetPreSignedUrlAsync(
                userDto.ProfileImageUrl,
                TimeSpan.FromDays(5),
                cancellationToken);
        }

        await cacheService.SetAsync(cacheKey,userDto,TimeSpan.FromMinutes(30),cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:",cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}