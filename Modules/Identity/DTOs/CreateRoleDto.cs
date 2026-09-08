namespace CampusServicePortal.Modules.Identity.DTOs;

public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }
}