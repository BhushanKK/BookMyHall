using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdatePaymentModeCommandValidator: AbstractValidator<UpdatePaymentModeCommand>
{
    public UpdatePaymentModeCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.PaymentModeId)
            .Required(localizer, EntityKeys.PaymentModeId);

        RuleFor(x => x.PaymentModeName)
            .Required(localizer, EntityKeys.PaymentModeName)
            .MaximumLength(100);
    }
}