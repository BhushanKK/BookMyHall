using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Options;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository, IRoleRepository roleRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository, IUnitOfWork unitOfWork,
    IMapper mapper, ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher, IMessageHelper messageHelper,
    ICacheService cacheService, IMessagePublisher messagePublisher,
    IOptions<EmailOptions> emailVerificationOptions)
    : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Roles is null || request.Roles.Count == 0)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                "At least one role is required.",
                HttpStatusCode.BadRequest
            );
        }

        var roleIds = request.Roles
            .Where(roleId => roleId != Guid.Empty)
            .Distinct()
            .ToArray();

        if (roleIds.Length == 0)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                "At least one valid role is required.",
                HttpStatusCode.BadRequest
            );
        }

        var roles = await roleRepository.GetByIdsAsync(roleIds, cancellationToken);

        if (roles.Count != roleIds.Length)
        {
            var existingRoleIds = roles.Select(role => role.RoleId).ToHashSet();
            var invalidRoleId = roleIds.First(roleId => !existingRoleIds.Contains(roleId));

            return ApiResponse<UserDto>.FailureResponse
            (
                $"Role with ID '{invalidRoleId}' was not found.",
                HttpStatusCode.BadRequest
            );
        }

        var expiryMinutes = emailVerificationOptions.Value.VerificationExpiryMinutes;

        if (expiryMinutes <= 0)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                "Email verification expiry configuration is invalid.",
                HttpStatusCode.InternalServerError
            );
        }

        var currentDate = DateTimeOffset.UtcNow;
        var user = mapper.Map<User>(request);

        user.IsEmailVerified = false;

        user.UserRoles = roles.Select(role => new UserRole
        {
            UserId = user.UserId,
            RoleId = role.RoleId,
            CreatedDate = currentDate,
            CreatedBy = user.CreatedBy
        }).ToList();

        try
        {
            await userRepository.AddAsync(user, cancellationToken);
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

        var verificationToken = tokenGenerator.GenerateEmailVerificationToken();
        var tokenHash = tokenHasher.Hash(verificationToken);
        var expiresAt = currentDate.AddMinutes(expiryMinutes);

        var verificationTokenEntity = EmailVerificationToken.Create(user.UserId, tokenHash, expiresAt);
        await emailVerificationTokenRepository.AddAsync(verificationTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);

        var registrationMessage = new UserRegisteredMessage
        (
            user.UserId,
            user.FullName,
            user.EmailAddress!,
            verificationToken,
            expiryMinutes
        );

        await messagePublisher.PublishAsync(registrationMessage, cancellationToken);

        var userDto = mapper.Map<UserDto>(user);

        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.Created
        );
    }
}