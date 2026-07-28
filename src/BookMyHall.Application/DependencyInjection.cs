using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BookMyHall.Application.Behaviors;

namespace BookMyHall.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.AddAutoMapper(config =>
        {
            config.AddMaps(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        return services;
    }
}