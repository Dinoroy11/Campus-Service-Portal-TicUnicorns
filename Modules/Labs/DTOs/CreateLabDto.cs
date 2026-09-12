namespace CampusServicePortal.Modules.Labs.DTOs;

public class CreateLabDto
{
    public string LabName { get; set; } = string.Empty;

    // Science or Computer
    public string LabType { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string Description { get; set; } = string.Empty;
}
