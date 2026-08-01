using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.CurrentPassword)
            .Required(localizer, EntityKeys.CurrentPassword);

        RuleFor(x => x.NewPassword)
            .Required(localizer, EntityKeys.NewPassword)
            .MinimumLengthLocalized(localizer, EntityKeys.NewPassword, 8)
            .MaximumLengthLocalized(localizer, EntityKeys.NewPassword, 100)
            .StrongPasswordLocalized(localizer);

        RuleFor(x => x.ConfirmPassword)
            .Required(localizer, EntityKeys.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "Equal",
                localizer.Get(ResourceNames.Entities, EntityKeys.ConfirmPassword),
                localizer.Get(ResourceNames.Entities, EntityKeys.NewPassword)));

        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "NotEqual",
                localizer.Get(ResourceNames.Entities, EntityKeys.NewPassword),
                localizer.Get(ResourceNames.Entities, EntityKeys.CurrentPassword)));
    }
}