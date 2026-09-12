namespace CampusServicePortal.Modules.Labs.DTOs;

public class LabDto
{
    public int LabId { get; set; }

    public string LabName { get; set; } = string.Empty;

    public string LabType { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
