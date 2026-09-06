using System.Security.Claims;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken(
        [FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);

        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto dto)
    {
        var userId = GetCurrentUserId();

        await _authService.ChangePasswordAsync(userId, dto);

        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequestDto dto)
    {
        await _authService.ForgotPasswordAsync(dto);

        return Ok(new
        {
            message = "If the account exists, password reset instructions will be provided."
        });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDto dto)
    {
        await _authService.ResetPasswordAsync(dto);

        return NoContent();
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequestDto dto)
    {
        await _authService.VerifyOtpAsync(dto);

        return Ok(new
        {
            message = "OTP verified successfully."
        });
    }

    [HttpPost("resend-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequestDto dto)
    {
        await _authService.ResendOtpAsync(dto);

        return Ok(new
        {
            message = "OTP sent successfully."
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = GetCurrentUserId();

        await _authService.LogoutAsync(userId);

        return NoContent();
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = GetCurrentUserId();

        await _authService.LogoutAllAsync(userId);

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return userId;
    }
}