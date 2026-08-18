using AutoMapper;

using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class UserPreferenceMappingProfile : Profile
{
    public UserPreferenceMappingProfile()
    {
        CreateMap<UpsertUserPreferenceCommand, UserPreference>()
    .ForMember(
        dest => dest.UserPreferenceId,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.UserId,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.CreatedDate,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.CreatedBy,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.UpdatedDate,
        opt => opt.Ignore())
    .ForMember(
        dest => dest.UpdatedBy,
        opt => opt.Ignore());

        CreateMap<UserPreference, UserPreferenceDto>();
    }
}