using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // var validationResult = await validator.ValidateAsync(request, cancellationToken);

        // if (!validationResult.IsValid)
        // {
        //     return ApiResponse<UserDto>.FailureResponse
        //     (
        //         string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
        //         HttpStatusCode.BadRequest
        //     );
        // }

        if (request.roles is null || !request.roles.Any())
        {
            return ApiResponse<UserDto>.FailureResponse(
                "At least one role is required.",
                HttpStatusCode.BadRequest);
        }

        var roleIds = request.roles
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

        if (!roleIds.Any())
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                "At least one valid role is required.",
                HttpStatusCode.BadRequest
            );
        }

        var roles = new List<Role>();

        foreach (var roleId in roleIds)
        {
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);

            if (role is null)
            {
                return ApiResponse<UserDto>.FailureResponse
                (
                    $"Role with ID '{roleId}' was not found.",
                    HttpStatusCode.BadRequest
                );
            }
            roles.Add(role);
        }

        var currentDate = DateTimeOffset.UtcNow;
        var user = mapper.Map<User>(request);

        user.UserRoles = roles.Select(role =>
        new UserRole
        {
            RoleId = role.RoleId,
            CreatedDate = currentDate,
            CreatedBy = user.CreatedBy
        }).ToList();

        try
        {
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // await mediator.Publish
            // (
            //     new UserRegisteredEvent(
            //         user.UserId,
            //         user.FullName,
            //         user.EmailAddress),
            //     cancellationToken
            // );   ---- Temp Commented need to work after rabbitMq implementation.
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<UserDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.Conflict);
        }

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse
        (
            mapper.Map<UserDto>(user),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.Created
        );
    }
}