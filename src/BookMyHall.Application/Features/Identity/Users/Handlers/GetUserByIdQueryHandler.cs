using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    IR2StorageService storageService,ICacheService cacheService)
    : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
       var cacheKey = $"{CacheKeys.Users}:{request.UserId}";
        var cachedUser = await cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);

        if (cachedUser is not null)
        {
            return ApiResponse<UserDto>.SuccessResponse(cachedUser, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.CancellationPolicy), HttpStatusCode.OK);
        }
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        var userDto = mapper.Map<UserDto>(user);
        
        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            userDto.ProfileImageUrl = await storageService.GetPreSignedUrlAsync
            (
                user.ProfileImageUrl,TimeSpan.FromMinutes(15),
                cancellationToken
            );
        }
        var response = mapper.Map<UserDto>(user);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}