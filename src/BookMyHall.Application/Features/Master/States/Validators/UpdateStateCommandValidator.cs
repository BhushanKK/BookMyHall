
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

using FluentValidation;

public sealed class UpdateStateCommandValidator: AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.StateId)
            .Required(localizer, EntityKeys.StateId);

            RuleFor(x => x.CountryId)
            .Required(localizer,EntityKeys.CountryId);

        RuleFor(x => x.StateName)
            .Required(localizer, EntityKeys.State)
            .MaximumLengthLocalized(localizer, EntityKeys.State, 20);
    }
}