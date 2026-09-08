namespace CampusServicePortal.Modules.Identity.Entities;

public class Permission
{
    public int PermissionId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Module { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    // Navigation property
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}