using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Persistence.Context;
using BookMyHall.Persistence.Repositories;
using BookMyHall.Persistence.Repositories.Identity;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Persistence.Repositories.Audit;
using BookMyHall.Infrastructure.Authentication;

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
        services.AddScoped<IPaymentModeRepository, PaymentModeRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}