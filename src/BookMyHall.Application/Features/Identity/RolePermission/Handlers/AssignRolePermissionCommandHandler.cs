using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class AssignRolePermissionCommandHandler(
    IRolePermissionRepository rolePermissionRepository,
    IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<AssignRolePermissionCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<AssignRolePermissionCommand,ApiResponse<RolePermissionDto>>
{
    public async Task<ApiResponse<RolePermissionDto>> Handle(
        AssignRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<RolePermissionDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var rolePermission = mapper.Map<RolePermission>(request);

        try
        {
            await rolePermissionRepository.AddAsync(rolePermission,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<RolePermissionDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.RolePermission),
                HttpStatusCode.Conflict);
        }

        var response = mapper.Map<RolePermissionDto>(rolePermission);

        return ApiResponse<RolePermissionDto>.SuccessResponse(response,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.RolePermission),HttpStatusCode.Created);
    }
}