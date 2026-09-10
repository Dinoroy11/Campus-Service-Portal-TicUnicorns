namespace CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

public class StudentOtpVerificationDto
{
    public string UniversityStudentId { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}