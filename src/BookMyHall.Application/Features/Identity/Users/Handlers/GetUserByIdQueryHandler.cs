using MediatR;

using System.Net;

using AutoMapper;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    IR2StorageService storageService)
    : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {

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
        
        var preSignedUrl = await storageService.GetPreSignedUrlAsync(
        userDto.ProfileImageUrl!,
        TimeSpan.FromMinutes(15),
        cancellationToken);
        
        userDto.ProfileImageUrl = preSignedUrl;
        
        return ApiResponse<UserDto>.SuccessResponse
        (
            userDto,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}