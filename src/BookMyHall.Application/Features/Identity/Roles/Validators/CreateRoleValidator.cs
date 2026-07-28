using FluentValidation;
using BookMyHall.Contracts.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.RoleName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(string.Format(ValidationMessages.Required, Entities.Role))
            .MaximumLength(20)
                .WithMessage(string.Format(ValidationMessages.MaxLength, Entities.Role, 20));
    }
}

public sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.Required, $"{Entities.Role} Id"));

        RuleFor(x => x.RoleName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.Required, Entities.Role))
            .MaximumLength(20)
            .WithMessage(string.Format(ValidationMessages.MaxLength, Entities.Role, 20));
    }
}

public sealed class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.Required, $"{Entities.Role} Id"));
    }
}