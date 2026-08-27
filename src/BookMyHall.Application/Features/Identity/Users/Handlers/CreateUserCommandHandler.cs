using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Identity;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IMapper mapper,
    IValidator<CreateUserCommand> validator,
    IMessageHelper messageHelper,
    IMediator mediator,ICacheService cacheService)
    : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        Guid roleId = await roleRepository.GetRoleIdByRoleName("Customer",cancellationToken);
        var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

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
            await mediator.Publish
            (
                new UserRegisteredEvent(user.UserId,user.FullName,user.EmailAddress),
                cancellationToken
            );
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.User),
                HttpStatusCode.Conflict
            );
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);
        return ApiResponse<UserDto>.SuccessResponse
        (
            mapper.Map<UserDto>(user),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.User),
            HttpStatusCode.Created
        );
    }
}