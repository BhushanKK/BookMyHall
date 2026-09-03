


using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        // -------------------------------------------------------
        // Get User
        // -------------------------------------------------------

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

        // -------------------------------------------------------
        // Validate Roles
        // -------------------------------------------------------

        var roleIds = request.Roles?
            .Where(roleId => roleId != Guid.Empty)
            .Distinct()
            .ToList();

        if (roleIds is null || roleIds.Count == 0)
        {
            return ApiResponse<UserDto>.FailureResponse(
                "At least one valid role is required.",
                HttpStatusCode.BadRequest);
        }

        var roles = new List<Role>();

        foreach (var roleId in roleIds)
        {
            var role = await roleRepository.GetByIdAsync(
                roleId,
                cancellationToken);

            if (role is null)
            {
                return ApiResponse<UserDto>.FailureResponse(
                    $"Role with ID '{roleId}' was not found.",
                    HttpStatusCode.BadRequest);
            }

            roles.Add(role);
        }

        // -------------------------------------------------------
        // Check Duplicate Email
        // -------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            var existingUser =
                await userRepository.GetByEmailAddressAsync(
                    request.EmailAddress,
                    cancellationToken);

            if (existingUser is not null &&
                existingUser.UserId != user.UserId)
            {
                return ApiResponse<UserDto>.FailureResponse(
                    messageHelper.AlreadyExistsEntity(
                        ResourceNames.Entities,
                        EntityKeys.User),
                    HttpStatusCode.Conflict);
            }
        }

        // -------------------------------------------------------
        // Update User
        // -------------------------------------------------------

        user.UpdateUserProfile(
            firstName: request.FirstName,
            middleName: request.MiddleName,
            lastName: request.LastName,
            mobileNumber: request.MobileNumber,
            emailAddress: request.EmailAddress ?? string.Empty);

        // -------------------------------------------------------
        // Update Active Status
        // -------------------------------------------------------

        if (request.IsActive)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        // -------------------------------------------------------
        // Update User Roles
        // -------------------------------------------------------

        await userRepository.RemoveUserRolesAsync(
            user.UserId,
            cancellationToken);

        var currentDate = DateTimeOffset.UtcNow;

        var userRoles = roles
       .Select(role => new UserRole
       {
           UserId = user.UserId,
           RoleId = role.RoleId,
           Role = role,
           CreatedDate = currentDate,
           CreatedBy = user.UpdatedBy
       })
       .ToList();

        foreach (var userRole in userRoles)
        {
            await userRepository.AddUserRoleAsync(userRole, cancellationToken);
        }


        // -------------------------------------------------------
        // Save Changes
        // -------------------------------------------------------

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // -------------------------------------------------------
        // Response
        // -------------------------------------------------------

        var userDto = mapper.Map<UserDto>(user);

        return ApiResponse<UserDto>.SuccessResponse(
            userDto,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.User),
            HttpStatusCode.OK);
    }
}

