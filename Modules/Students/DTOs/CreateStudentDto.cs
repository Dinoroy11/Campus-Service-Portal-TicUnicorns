namespace CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

public class CreateStudentDto
{
    public int MasterStudentId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
}
