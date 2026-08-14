using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;
public sealed class DeleteHallPricingCommandValidator:AbstractValidator<DeleteHallPricingCommand>
{
    public DeleteHallPricingCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallPricingId)
        .Required(localizer, EntityKeys.HallPricingId);
    }
}