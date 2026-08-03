using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdateAreaCommandValidator: AbstractValidator<UpdateAreaCommand>
{
    public UpdateAreaCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.AreaId)
            .Required(localizer, EntityKeys.AreaId);

        RuleFor(x => x.AreaName)
            .Required(localizer, EntityKeys.AreaName)
            .MaximumLengthLocalized(localizer, EntityKeys.Area, 150);

        RuleFor(x => x.Pincode)
            .Required(localizer, EntityKeys.Pincode)
            .Length(6);

        RuleFor(x => x.CityId)
            .Required(localizer, EntityKeys.CityId);
    }
}