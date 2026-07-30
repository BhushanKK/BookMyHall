using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

using FluentValidation;

namespace BookMyHall.Application.Validations;

public sealed class CreateRoleCommandValidator
    : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.RoleName)
            .Required(localizer, EntityKeys.Role)
            .MaximumLengthLocalized(localizer, EntityKeys.Role, 100);
    }
}

public sealed class UpdateRoleCommandValidator
    : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.RoleId)
            .Required(localizer, EntityKeys.RoleId);

        RuleFor(x => x.RoleName)
            .Required(localizer, EntityKeys.Role)
            .MaximumLengthLocalized(localizer, EntityKeys.Role, 100);
    }
}