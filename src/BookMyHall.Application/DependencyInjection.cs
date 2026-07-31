using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}