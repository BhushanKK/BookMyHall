using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;
public sealed class CreateRoleCommandHandler(IRoleRepository roleRepository,IUnitOfWork unitOfWork,
    IMapper mapper,IValidator<CreateRoleCommand> validator,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<RoleDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

       var roleName = request.RoleName.Trim();
        var existingRole =await roleRepository.GetByNameIncludingDeletedAsync(roleName,cancellationToken);

        // Active record already exists.
        if (existingRole is not null && !existingRole.IsDeleted)
        {
            return ApiResponse<RoleDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.Role),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingRole is not null &&
            existingRole.IsDeleted)
        {
            existingRole.IsDeleted = false;
            existingRole.IsActive = true;
            existingRole.RoleName =roleName;

            await roleRepository.UpdateAsync(existingRole,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingRole.RoleId,cancellationToken);

            return ApiResponse<RoleDto>.SuccessResponse(mapper.Map<RoleDto>(existingRole),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Role),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var role = mapper.Map<Role>(request);
        role.RoleId = Guid.NewGuid();
        role.RoleName = roleName;
        role.IsActive = true;
        role.IsDeleted = false;

        try
        {
            await roleRepository.AddAsync(role,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<RoleDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.Role),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(role.RoleId,cancellationToken);
        return ApiResponse<RoleDto>.SuccessResponse(mapper.Map<RoleDto>(role),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Role),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid roleId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.Roles}:{roleId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.RolesPaged}:",cancellationToken);
    }
}