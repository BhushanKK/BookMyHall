using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class RefreshTokenCommandValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.RefreshToken)
            .Required(localizer, "RefreshToken");
    }
}