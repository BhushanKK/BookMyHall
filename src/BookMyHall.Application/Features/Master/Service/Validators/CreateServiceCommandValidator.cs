using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateServiceCommandValidator: AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.ServiceName)
            .Required(localizer, EntityKeys.ServiceName)
            .MaximumLength(100);

        RuleFor(x => x.ServiceIcon)
            .Required(localizer, EntityKeys.ServiceIcon)
            .MaximumLength(500);
    }
}