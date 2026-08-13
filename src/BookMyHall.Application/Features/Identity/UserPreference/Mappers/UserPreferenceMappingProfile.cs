using AutoMapper;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class UserPreferenceMappingProfile : Profile
{
    public UserPreferenceMappingProfile()
    {
        CreateMap<UserPreference, UserPreferenceDto>();

        CreateMap<UpdateUserPreferenceCommand, UserPreference>()
            .ForMember(dest => dest.UserPreferenceId,opt => opt.Ignore())
            .ForMember(dest => dest.UserId,opt => opt.Ignore());
    }
}