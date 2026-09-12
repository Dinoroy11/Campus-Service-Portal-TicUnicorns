using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.Labs.DTOs;

public class UpdateLabSeatStatusDto
{
    // Allowed values: Available, Maintenance, Inactive
    [Required]
    public string Status { get; set; } = string.Empty;

    // Used in affected-student notifications.
    // This is not stored on LabSeats in the current database schema.
    public string? Reason { get; set; }
}