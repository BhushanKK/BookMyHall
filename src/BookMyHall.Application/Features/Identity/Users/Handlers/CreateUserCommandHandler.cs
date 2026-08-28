using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class CreateUserCommandHandler(IUserRepository userRepository,
IRoleRepository roleRepository,IUnitOfWork unitOfWork,IMapper mapper,
IMessageHelper messageHelper,ICacheService cacheService): IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request,CancellationToken cancellationToken)
{
   if (request.Roles is null || request.Roles.Count == 0)
     {
     return ApiResponse<UserDto>.FailureResponse("At least one role is required.",HttpStatusCode.BadRequest);
     }

    var roleIds = request.Roles
        .Where(roleId => roleId != Guid.Empty)
        .Distinct()
        .ToList();

    if (roleIds.Count == 0)
    {
        return ApiResponse<UserDto>.FailureResponse("At least one valid role is required.",HttpStatusCode.BadRequest);
    }

    var roles = new List<Role>();

    foreach (var roleId in roleIds)
    {
        var role = await roleRepository.GetByIdAsync(roleId,cancellationToken);
        if (role is null)
        {
            return ApiResponse<UserDto>.FailureResponse($"Role with ID '{roleId}' was not found.",
                HttpStatusCode.BadRequest);
        }
        roles.Add(role);
    }

    var currentDate = DateTimeOffset.UtcNow;
    var user = mapper.Map<User>(request);
    user.UserRoles = roles.Select(role => new UserRole
    {
        RoleId = role.RoleId,
        CreatedDate = currentDate,
        CreatedBy = user.CreatedBy
    }).ToList();

    try
    {
        await userRepository.AddAsync(user,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    catch (DuplicateRecordException)
    {
        return ApiResponse<UserDto>.FailureResponse(messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                EntityKeys.User),HttpStatusCode.Conflict);
    }

    await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:",cancellationToken);

    return ApiResponse<UserDto>.SuccessResponse(mapper.Map<UserDto>(user),
        messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.User),
        HttpStatusCode.Created);
}
}
