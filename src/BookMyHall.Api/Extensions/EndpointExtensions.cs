using BookMyHall.Api.Endpoints.Identity;
using BookMyHall.Api.Endpoints.Master;
using BookMyHall.Api.Endpoints.Menu;
using BookMyHall.Api.Endpoints.Role;
using BookMyHall.Api.Endpoints.Venue;
namespace BookMyHall.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapBookMyHallEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRoleEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapAuthenticationEndpoints();
        endpoints.MapDeviceEndpoints();
        endpoints.MapStateEndpoints();
        endpoints.MapAmenityEndpoints();
        endpoints.MapAreaEndpoints();
        endpoints.MapCancellationPolicyEndpoints();
        endpoints.MapCityEndpoints();
        endpoints.MapDistrictEndpoints();
        endpoints.MapCountryEndpoints();
        endpoints.MapEventCategoryEndpoints();
        endpoints.MapFacilityEndpoints();
        endpoints.MapFoodTypeEndpoints();
        endpoints.MapPaymentModeEndpoints();
        endpoints.MapServiceEndpoints();
        endpoints.MapUserPreferenceEndpoints();
        endpoints.MapHallEndpoints();
        endpoints.MapHallPricingEndpoints();
        endpoints.MapHallCategoryEndpoints();
        endpoints.MapHallBlockEndpoints();
        endpoints.MapHallImageEndpoints();
        endpoints.MapPermissionEndpoints();
        endpoints.MapRolePermissionEndpoints();
        endpoints.MapMenuEndpoints();

        return endpoints;
    }
}
