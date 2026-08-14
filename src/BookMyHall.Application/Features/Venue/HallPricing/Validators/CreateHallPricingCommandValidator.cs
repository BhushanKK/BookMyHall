using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateHallPricingCommandValidator: AbstractValidator<CreateHallPricingCommand>
{
    public CreateHallPricingCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallId)
            .Required(localizer, EntityKeys.HallId);

        RuleFor(x => x.EventCategoryId)
            .Required(localizer, EntityKeys.EventCategoryId);

        RuleFor(x => x.PackageName)
            .Required(localizer, EntityKeys.PackageName)
            .MaximumLength(150);

        RuleFor(x => x.MinimumGuests)
            .GreaterThanOrEqualTo(1)
            .When(x => x.MinimumGuests.HasValue);

        RuleFor(x => x.MaximumGuests)
            .GreaterThanOrEqualTo(1)
            .When(x => x.MaximumGuests.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinimumGuests.HasValue ||
                !x.MaximumGuests.HasValue ||
                x.MinimumGuests <= x.MaximumGuests)
            .WithMessage("Minimum guests must be less than or equal to maximum guests.");

        RuleFor(x => x.WeekdayPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.WeekdayPrice.HasValue);

        RuleFor(x => x.WeekendPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.WeekendPrice.HasValue);

        RuleFor(x => x.AdvanceAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AdvanceAmount.HasValue);

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SecurityDeposit.HasValue);

        RuleFor(x => x.ExtraGuestCharge)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ExtraGuestCharge.HasValue);
    }
}