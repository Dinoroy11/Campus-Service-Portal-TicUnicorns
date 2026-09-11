using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

public interface IStudentRegistrationService
{
    Task SendOtpAsync(
        StudentRegistrationDto dto);

    Task<LoginResponseDto> VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp);
}