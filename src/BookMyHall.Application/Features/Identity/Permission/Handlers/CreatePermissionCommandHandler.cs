using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Identity;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreatePermissionCommandHandler(
    IPermissionRepository permissionRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreatePermissionCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreatePermissionCommand, ApiResponse<PermissionDto>>
{
    public async Task<ApiResponse<PermissionDto>> Handle(CreatePermissionCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<PermissionDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var permission = mapper.Map<Permission>(request);
        permission.PermissionId = Guid.NewGuid();
        permission.IsActive = true;

        try
        {
            await permissionRepository.AddAsync(permission,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<PermissionDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync(CacheKeys.PermissionPaged,cancellationToken);
        
        return ApiResponse<PermissionDto>.SuccessResponse( mapper.Map<PermissionDto>(permission),messageHelper.AddedEntity(
                ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.Created);
    }
}

