namespace BookMyHall.Application.Abstractions.Caching;

public static class CacheKeys
{
    public const string Amenity="amenity";
    public const string AmenitiesPaged="amenities:page";
    public const string Country = "country";
    public const string CountriesPaged = "countries:page:";
    public const string States = "states";
    public const string StatesPaged = "states:page:";
    public const string StateByCode="state:code";
    public const string StateByName = "state:name";
    public const string Districts = "districts";
    public const string DistrictsPaged = "districts:page:";
    public const string Cities = "cities";
    public const string CitiesPaged = "cities:page:";
    public const string Areas = "areas";
    public const string AreasPaged = "areas:page:";
    public const string CancellationPolicies = "cancelationpolicies";
    public const string CancellationPoliciesPaged = "cancelationpolicies:page:";
    public const string EventCategories = "eventcategories";
    public const string EventCategoriesPaged = "eventcategories:page:";
    public const string Facilities = "facilities";
    public const string FacilitiesPaged = "facilities:page:";
    public const string Foodtype = "foodtype";
    public const string FoodtypePaged = "foodtype:page:";
    public const string HallCategories = "hallcategories";
    public const string HallCategoriesPaged = "hallcategories:page:";
    public const string PaymentMode = "paymentmode";
    public const string PaymentModesPaged = "paymentmodes:page:";
    public const string Services = "services";
    public const string ServicesPaged = "services:page:";

    public const string Hall = "hall";
     public const string HallsPaged = "halls:page:";
    public const string HallImage = "hallimage";
     public const string HallImagesPaged = "hallimages:page:";
    public const string HallBlock = "hallblock:";
    public const string HallBlocksPaged = "hallblocks:paged:";
    public const string HallPricing = "hallpricing";
    public const string HallPricingsPaged = "hallpricings:page:";
    public const string HallCoverImage = "HallCoverImage";
    public const string HallCoverImagesPaged = "HallCoverImages:page:";
    public const string HallOwner="HallOwner";
    public const string Roles = "roles";
    public const string RolesPaged = "roles:page:";
    public const string Users = "users";
    public const string UsersPaged = "users:page:";
    public const string RolePermissions = "rolepermissions";
    public const string RolePermissionPaged = "rolepermissions:page:";
    public const string Permissions = "permissions";
    public const string PermissionPaged = "permissions:page:";
    public const string Menus = "menus";
    public const string MenuPaged = "menu:page:";
    public const string MenuRolePermissionPaged="menuRolePermissionPaged";
    public const string MenuRolePermission ="menuRolePermission";
    public const string Devices = "devices";
    public const string DevicePaged = "device:page:";

//-----------Location Lookups Cached Keys-------------//
    public const string CountriesCached ="location:countries";
    public const string StatesCached="location:states:countryId";
    public const string DistrictsCached="location:districts:stateId";
    public const string CitiesCached ="location:cities:districtId";
    public const string AreasCached="location:areas:cityId";

//-----------Masters AutoComplete Cached Keys-------------//
    public const string AmenitiesAutoComplete="AmenitiesAutoComplete";
    public const string AreasAutoComplete="AreasAutoComplete";
    public const string CancellationPoliciesAutoComplete="CancellationPoliciesAutoComplete";
    public const string CitiesAutoComplete="CitiesAutoComplete";
    public const string CountriesAutoComplete="CountriesAutoComplete";
    public const string DistrictsAutoComplete="DistrictsAutoComplete";
    public const string EventCategoriesAutoComplete="EventCategoriesAutoComplete";
    public const string FacilitiesAutoComplete="FacilitiesAutoComplete";
    public const string FoodTypesAutoComplete="FoodTypesAutoComplete";
    public const string HallCategoriesAutoComplete="HallCategoriesAutoComplete";
    public const string PaymentModesAutoComplete="PaymentModesAutoComplete";
    public const string ServicesAutoComplete="ServicesAutoComplete";
    public const string StatesAutoComplete="StatesAutoComplete";
     public const string HallsAutoComplete="HallsAutoComplete";
}
