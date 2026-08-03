using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateStateCommandValidator: AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.StateName)
            .Required(localizer, EntityKeys.State)
            .MaximumLengthLocalized(localizer, EntityKeys.State, 20);
    }
}

public sealed class UpdateStateCommandValidator: AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.StateId)
            .Required(localizer, EntityKeys.StateId);

        RuleFor(x => x.StateName)
            .Required(localizer, EntityKeys.State)
            .MaximumLengthLocalized(localizer, EntityKeys.State, 20);
    }
}