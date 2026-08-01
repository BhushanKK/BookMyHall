using FluentValidation;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
using System.Linq.Expressions;

namespace BookMyHall.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Required<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        ILocalizationService localizer,
        string entityKey)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "Required",
                localizer.Get(ResourceNames.Entities, entityKey)));
    }

    public static IRuleBuilderOptions<T, Guid> Required<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        ILocalizationService localizer,
        string entityKey)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "Required",
                localizer.Get(ResourceNames.Entities, entityKey)));
    }

    public static IRuleBuilderOptions<T, string> MaximumLengthLocalized<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        ILocalizationService localizer,
        string entityKey,
        int length)
    {
        return ruleBuilder
            .MaximumLength(length)
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "MaximumLength",
                localizer.Get(ResourceNames.Entities, entityKey),
                length));
    }

    public static IRuleBuilderOptions<T, string> MinimumLengthLocalized<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        ILocalizationService localizer,
        string entityKey,
        int length)
    {
        return ruleBuilder
            .MinimumLength(length)
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "MinimumLength",
                localizer.Get(ResourceNames.Entities, entityKey),
                length));
    }

    public static IRuleBuilderOptions<T, string> EmailLocalized<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        ILocalizationService localizer)
    {
        return ruleBuilder
            .EmailAddress()
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "Email"));
    }

    public static IRuleBuilderOptions<T, string> PhoneLocalized<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        ILocalizationService localizer)
    {
        return ruleBuilder
            .Matches(@"^[6-9]\d{9}$")
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "PhoneNumber"));
    }

    public static IRuleBuilderOptions<T, TProperty> EqualToLocalized<T, TProperty>(
    this IRuleBuilder<T, TProperty> ruleBuilder,
    Expression<Func<T, TProperty>> expression,
    ILocalizationService localizer,
    string currentEntityKey,
    string compareEntityKey)
    {
        return ruleBuilder
            .Equal(expression)
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "Equal",
                localizer.Get(ResourceNames.Entities, currentEntityKey),
                localizer.Get(ResourceNames.Entities, compareEntityKey)));
    }

    public static IRuleBuilderOptions<T, string> StrongPasswordLocalized<T>(
    this IRuleBuilder<T, string> ruleBuilder,
    ILocalizationService localizer)
    {
        return ruleBuilder
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage(localizer.Get(
                ResourceNames.ValidationMessages,
                "StrongPassword"));
    }
}