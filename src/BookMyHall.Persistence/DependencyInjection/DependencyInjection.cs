using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Persistence.Context;
using BookMyHall.Persistence.Repositories;

namespace BookMyHall.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<BookMyHallDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFoodTypeRepository, FoodTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}