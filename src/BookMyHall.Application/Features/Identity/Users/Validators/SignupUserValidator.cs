using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateUserValidator
    : AbstractValidator<SignupUserCommand>
{
    public CreateUserValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.FirstName)
            .Required(localizer, EntityKeys.FirstName)
            .MaximumLengthLocalized(localizer, EntityKeys.FirstName, 100);


        RuleFor(x => x.MobileNumber)
            .Required(localizer, EntityKeys.MobileNumber)
            .MaximumLengthLocalized(localizer, EntityKeys.MobileNumber, 15);

        RuleFor(x => x.EmailAddress)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress));


        RuleFor(x => x.Password)
            .Required(localizer, EntityKeys.Password);

    }
}

public sealed class UpdateUserCommandValidator
    : AbstractValidator<ProfileUpdateUserCommand>
{
    public UpdateUserCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.UserId)
            .Required(localizer, EntityKeys.UserId);

        RuleFor(x => x.FirstName)
            .Required(localizer, EntityKeys.FirstName)
            .MaximumLengthLocalized(localizer, EntityKeys.FirstName, 100);

        RuleFor(x => x.MobileNumber)
            .Required(localizer, EntityKeys.MobileNumber)
            .MaximumLengthLocalized(localizer, EntityKeys.MobileNumber, 15);
            
        RuleFor(x => x.EmailAddress)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress));
    }
}