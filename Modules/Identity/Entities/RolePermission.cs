namespace CampusServicePortal.Modules.Identity.Entities;

public class RolePermission
{
    public int RolePermissionId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}