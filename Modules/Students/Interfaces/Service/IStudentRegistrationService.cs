using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

public interface IStudentRegistrationService
{
    Task SendOtpAsync(StudentRegistrationDto dto);

    Task VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp);
}