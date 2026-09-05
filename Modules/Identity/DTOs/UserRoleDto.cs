namespace CampusServicePortal.Modules.Identity.DTOs;

public class UserRoleDto
{
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; }
}