using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Identity;

public class MenuPermission 
{
    public Guid MenuPermissionId {get; set;}
    public Guid MenuId {get; set;}
    public Guid PermissionId {get; set;}
    public Menu Menu {get; set;} = null!;
    public Permission Permission {get;set;} = null!;
}