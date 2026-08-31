using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Options;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Shared.Common;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class SignUpUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IMapper mapper,
    IValidator<SignupUserCommand> validator,
    IMessageHelper messageHelper,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IMessagePublisher messagePublisher,
    ICacheService cacheService,
    IOptions<EmailOptions> emailOptions)
    : IRequestHandler<SignupUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        SignupUserCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Validate request
        // ------------------------------------------------------------

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 2. Get Customer role
        // ------------------------------------------------------------

        var roleId = await roleRepository.GetRoleIdByRoleName("Customer", cancellationToken);
        var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        // ------------------------------------------------------------
        // 3. Validate email verification configuration
        // ------------------------------------------------------------

        var verificationExpiryMinutes = emailOptions.Value.VerificationExpiryMinutes;

        if (verificationExpiryMinutes <= 0)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                "Email verification expiry configuration is invalid.",
                HttpStatusCode.InternalServerError
            );
        }

        // ------------------------------------------------------------
        // 4. Create user
        // ------------------------------------------------------------

        var currentDate = DateTimeOffset.UtcNow;

        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.HashPassword(request.Password);

        user.UserRoles =
        [
            new UserRole
            {
                RoleId = role.RoleId,
                CreatedDate = currentDate,
                CreatedBy = user.CreatedBy
            }
        ];

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

        // ------------------------------------------------------------
        // 5. Generate email verification token
        // ------------------------------------------------------------

        var verificationToken = tokenGenerator.GenerateEmailVerificationToken();
        var tokenHash = tokenHasher.Hash(verificationToken);
        var expiresAt = currentDate.AddMinutes(verificationExpiryMinutes);

        var verificationTokenEntity = EmailVerificationToken.Create
        (
            user.UserId,
            tokenHash,
            expiresAt
        );

        await emailVerificationTokenRepository.AddAsync(verificationTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // 6. Publish RabbitMQ registration message
        // ------------------------------------------------------------

        var registrationMessage = new UserRegisteredMessage
        (
            UserId: user.UserId,
            FullName: user.FullName,
            EmailAddress: user.EmailAddress,
            VerificationToken: verificationToken,
            ExpiryMinutes: verificationExpiryMinutes
        );

        await messagePublisher.PublishAsync(registrationMessage, cancellationToken);
    
        // ------------------------------------------------------------
        // 7. Clear cache
        // ------------------------------------------------------------

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);

        // ------------------------------------------------------------
        // 8. Return response
        // ------------------------------------------------------------

        return ApiResponse<UserDto>.SuccessResponse
        (
            mapper.Map<UserDto>(user),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.Created
        );
    }
}