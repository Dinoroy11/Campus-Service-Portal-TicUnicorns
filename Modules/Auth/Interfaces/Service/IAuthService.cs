using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto dto);

    Task<LoginResponseDto> CreateLoginResponseAsync(
        User user,
        string roleName);

    Task<RefreshTokenResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto dto);

    Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto dto);

    Task ForgotPasswordAsync(
        ForgotPasswordRequestDto dto);

    Task ResetPasswordAsync(
        ResetPasswordRequestDto dto);

    Task VerifyOtpAsync(
        VerifyOtpRequestDto dto);

    Task ResendOtpAsync(
        ResendOtpRequestDto dto);

    Task LogoutAsync(
        int userId);

    Task LogoutAllAsync(
        int userId);
}