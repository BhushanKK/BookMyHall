using MediatR;
using System.Net;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository, IMessageHelper messageHelper,
    IR2StorageService storageService, ICacheService cacheService)
    : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Users}:{request.UserId}";
        var cachedUser = await cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);

        if (cachedUser is not null)
        {
            return ApiResponse<UserDto>.SuccessResponse
            (
                cachedUser,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.OK
            );
        }

        var userDto = await userRepository.GetUserDtoByIdAsync(request.UserId, cancellationToken);
        if (userDto is null)
        {
            return ApiResponse<UserDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        if (!string.IsNullOrWhiteSpace(userDto.ProfileImageUrl))
        {
            userDto.ProfileImageUrl = await storageService.GetPreSignedUrlAsync
            (
                userDto.ProfileImageUrl,
                TimeSpan.FromDays(5),
                cancellationToken
            );
        }

        await cacheService.SetAsync( cacheKey, userDto, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}