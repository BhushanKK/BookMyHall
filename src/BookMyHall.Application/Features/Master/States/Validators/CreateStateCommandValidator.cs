using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

using FluentValidation;

public sealed class CreateStateCommandValidator: AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.CountryId)
            .Required(localizer,EntityKeys.CountryId);

        RuleFor(x => x.StateName)
            .Required(localizer,EntityKeys.State)
            .MaximumLengthLocalized(localizer,EntityKeys.State,20);
    }
}