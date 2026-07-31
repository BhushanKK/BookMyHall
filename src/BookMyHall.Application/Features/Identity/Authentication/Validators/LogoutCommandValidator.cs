using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator(ILocalizationService localizer)
        => RuleFor(x => x.RefreshToken).Required(localizer, "RefreshToken");
}