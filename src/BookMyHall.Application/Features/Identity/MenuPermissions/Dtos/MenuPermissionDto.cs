using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class MenuPermissionDto
{
    [JsonIgnore]
    public Guid MenuPermissionId {get;set;}
    public Guid MenuId {get;set;}
    public Guid PermissionId{get;set;}
}