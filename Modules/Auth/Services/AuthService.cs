using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IOtpService _otpService;

    public AuthService(
        IAuthRepository authRepository,
        IConfiguration configuration,
        IOtpService otpService)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _otpService = otpService;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto dto)
    {
        var user = await _authRepository
            .GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
            GetAccessTokenLifetimeMinutes());

        var accessToken = GenerateAccessToken(
            user,
            accessTokenExpiresAt);

        var refreshToken = GenerateRefreshToken();

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            MustChangePassword = user.MustChangePassword,
            IsPhoneVerified = user.IsPhoneVerified
        };
    }

    public Task<RefreshTokenResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto dto)
    {
        throw new NotSupportedException(
            "Refresh token persistence is not available until the shared authentication persistence is implemented.");
    }

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto dto)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.CurrentPassword);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.NewPassword);

        user.MustChangePassword = false;

        await _authRepository.UpdateUserAsync(user);
    }

    public Task ForgotPasswordAsync(
        ForgotPasswordRequestDto dto)
    {
        throw new NotSupportedException(
            "Password reset persistence is not available until the shared authentication persistence is implemented.");
    }

    public Task ResetPasswordAsync(
        ResetPasswordRequestDto dto)
    {
        throw new NotSupportedException(
            "Password reset persistence is not available until the shared authentication persistence is implemented.");
    }

    public async Task VerifyOtpAsync(
        VerifyOtpRequestDto dto)
    {
        var user = await _authRepository
            .GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        throw new NotSupportedException(
            "OTP persistence and verification storage are not available until the shared authentication persistence is implemented.");
    }

    public async Task ResendOtpAsync(
        ResendOtpRequestDto dto)
    {
        var user = await _authRepository
            .GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        throw new NotSupportedException(
            "OTP persistence and delivery are not available until the shared authentication persistence is implemented.");
    }

    public Task LogoutAsync(int userId)
    {
        throw new NotSupportedException(
            "Token revocation persistence is not available until the shared authentication persistence is implemented.");
    }

    public Task LogoutAllAsync(int userId)
    {
        throw new NotSupportedException(
            "Token revocation persistence is not available until the shared authentication persistence is implemented.");
    }

    private string GenerateAccessToken(
        User user,
        DateTime expiresAt)
    {
        var key = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured.");
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,
                user.UserId.ToString()),

            new(ClaimTypes.Name,
                user.Username),

            new(ClaimTypes.NameIdentifier,
                user.UserId.ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    private int GetAccessTokenLifetimeMinutes()
    {
        var configuredValue =
            _configuration["Jwt:AccessTokenLifetimeMinutes"];

        return int.TryParse(
            configuredValue,
            out var minutes)
            ? minutes
            : 60;
    }
}