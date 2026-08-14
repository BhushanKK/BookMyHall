using BookMyHall.Api.Endpoints.Identity;
using BookMyHall.Api.Endpoints.Master;
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
        endpoints.MapEventCategoryEndpoints();
        endpoints.MapFacilityEndpoints();
        endpoints.MapFoodTypeEndpoints();
        endpoints.MapPaymentModeEndpoints();
        endpoints.MapServiceEndpoints();
        endpoints.MapUserPreferenceEndpoints();
        endpoints.MapHallEndpoints();
        endpoints.MapHallPricingEndpoints();
        endpoints.MapHallCategoryEndpoints();
        return endpoints;
    }
}
