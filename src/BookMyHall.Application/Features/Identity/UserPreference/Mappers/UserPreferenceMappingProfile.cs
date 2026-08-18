using AutoMapper;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class UserPreferenceMappingProfile : Profile
{
    public UserPreferenceMappingProfile()
    {
        CreateMap<UserPreference, UserPreferenceDto>();

        CreateMap<CreateUserPreferenceCommand, UserPreference>()
            .ForMember(destination => destination.UserPreferenceId,options => options.Ignore());

        CreateMap<UpdateUserPreferenceCommand, UserPreference>()
            .ForMember(destination => destination.UserPreferenceId,options => options.Ignore())
            .ForMember(destination => destination.UserId,options => options.Ignore());
    }
}