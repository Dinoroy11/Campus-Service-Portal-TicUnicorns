namespace CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }

    public int MasterStudentId { get; set; }

    public int? UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime AdmissionDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}