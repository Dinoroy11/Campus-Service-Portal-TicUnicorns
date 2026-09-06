namespace CampusServicePortal.Modules.Identity.DTOs;

public class AuditLogDto
{
    public int AuditLogId { get; set; }

    public int UserId { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }
}