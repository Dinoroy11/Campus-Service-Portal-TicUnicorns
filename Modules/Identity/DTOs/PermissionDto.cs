namespace CampusServicePortal.Modules.Identity.DTOs;

public class PermissionDto
{
    public int PermissionId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Module { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}