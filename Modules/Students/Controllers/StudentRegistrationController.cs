using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentRegistrationController : ControllerBase
{
    private readonly IStudentRegistrationService
        _studentRegistrationService;

    public StudentRegistrationController(
        IStudentRegistrationService studentRegistrationService)
    {
        _studentRegistrationService =
            studentRegistrationService;
    }

    [HttpPost("send-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendOtp(
        [FromBody] StudentRegistrationDto dto)
    {
        await _studentRegistrationService.SendOtpAsync(dto);

        return Ok(new
        {
            message = "OTP sent successfully."
        });
    }
}