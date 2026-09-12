using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;

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

    private readonly IUserRoleRepository
        _userRoleRepository;

    private readonly IConfiguration
        _configuration;

    private readonly PasswordHasher<User>
        _passwordHasher;

    private readonly IOtpService
        _otpService;


    public AuthService(
        IAuthRepository authRepository,
        IUserRoleRepository userRoleRepository,
        IConfiguration configuration,
        IOtpService otpService)
    {
        _authRepository =
            authRepository;

        _userRoleRepository =
            userRoleRepository;

        _configuration =
            configuration;

        _otpService =
            otpService;

        _passwordHasher =
            new PasswordHasher<User>();
    }


    // =========================================================
    // NORMAL LOGIN
    // ADMIN + STUDENT
    // =========================================================

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(
                nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(
                dto.Username) ||
            string.IsNullOrWhiteSpace(
                dto.Password))
        {
            throw new UnauthorizedAccessException(
                "Username and password are required.");
        }

        // =====================================================
        // 1. FIND USER
        // =====================================================

        var user =
            await _authRepository
                .GetUserByUsernameAsync(
                    dto.Username);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        // =====================================================
        // 2. USER MUST BE ACTIVE
        // =====================================================

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        // =====================================================
        // 3. FIRST-TIME STUDENT MUST COMPLETE PASSWORD SETUP
        // =====================================================

        if (user.MustChangePassword)
        {
            throw new UnauthorizedAccessException(
                "Complete the initial password setup before login.");
        }

        // =====================================================
        // 4. VERIFY PASSWORD
        // =====================================================

        var passwordResult =
            _passwordHasher
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        // =====================================================
        // 5. GET ROLE
        // =====================================================

        var roleName =
            await _userRoleRepository
                .GetRoleNameByUserIdAsync(
                    user.UserId);

        if (string.IsNullOrWhiteSpace(
                roleName))
        {
            throw new UnauthorizedAccessException(
                "No active role is assigned to this user.");
        }

        var isAdmin =
            roleName.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        var isStudent =
            roleName.Equals(
                "Student",
                StringComparison.OrdinalIgnoreCase);

        // Current project phase:
        // only Admin and Student.
        if (!isAdmin &&
            !isStudent)
        {
            throw new UnauthorizedAccessException(
                "This user role is not currently supported.");
        }

        // Student must have verified mobile.
        if (isStudent &&
            !user.IsPhoneVerified)
        {
            throw new UnauthorizedAccessException(
                "Student mobile number has not been verified.");
        }

        // =====================================================
        // 6. CREATE ACCESS TOKEN
        // =====================================================

        var accessTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(
                GetAccessTokenLifetimeMinutes());

        var accessToken =
            GenerateAccessToken(
                user,
                accessTokenExpiresAt,
                roleName);

        // =====================================================
        // 7. CREATE REFRESH TOKEN
        // =====================================================

        var refreshToken =
            GenerateRefreshToken();

        // =====================================================
        // 8. RESPONSE
        // =====================================================

        return new LoginResponseDto
        {
            UserId =
                user.UserId,

            Username =
                user.Username,

            Role =
                roleName,

            AccessToken =
                accessToken,

            RefreshToken =
                refreshToken,

            AccessTokenExpiresAt =
                accessTokenExpiresAt,

            MustChangePassword =
                user.MustChangePassword,

            IsPhoneVerified =
                user.IsPhoneVerified
        };
    }


    // =========================================================
    // CREATE LOGIN RESPONSE
    // =========================================================

    public Task<LoginResponseDto>
        CreateLoginResponseAsync(
            User user,
            string roleName)
    {
        if (user is null)
        {
            throw new ArgumentNullException(
                nameof(user));
        }

        if (string.IsNullOrWhiteSpace(
                roleName))
        {
            throw new ArgumentException(
                "Role name is required.",
                nameof(roleName));
        }

        var accessTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(
                GetAccessTokenLifetimeMinutes());

        var accessToken =
            GenerateAccessToken(
                user,
                accessTokenExpiresAt,
                roleName);

        var refreshToken =
            GenerateRefreshToken();

        var response =
            new LoginResponseDto
            {
                UserId =
                    user.UserId,

                Username =
                    user.Username,

                Role =
                    roleName,

                AccessToken =
                    accessToken,

                RefreshToken =
                    refreshToken,

                AccessTokenExpiresAt =
                    accessTokenExpiresAt,

                MustChangePassword =
                    user.MustChangePassword,

                IsPhoneVerified =
                    user.IsPhoneVerified
            };

        return Task.FromResult(
            response);
    }


    // =========================================================
    // CREATE PASSWORD SETUP TOKEN
    // =========================================================

    public Task<string>
        CreatePasswordSetupTokenAsync(
            User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(
                nameof(user));
        }

        var key =
            _configuration[
                "Jwt:Key"];

        if (string.IsNullOrWhiteSpace(
                key))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured.");
        }

        var issuer =
            _configuration[
                "Jwt:Issuer"];

        var audience =
            _configuration[
                "Jwt:Audience"];

        // Valid for 10 minutes.
        var expiresAt =
            DateTime.UtcNow
                .AddMinutes(10);

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new(
                    ClaimTypes.Name,
                    user.Username),

                new(
                    "purpose",
                    "password_setup")
            };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer:
                    issuer,

                audience:
                    audience,

                claims:
                    claims,

                expires:
                    expiresAt,

                signingCredentials:
                    credentials);

        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return Task.FromResult(
            tokenString);
    }


    // =========================================================
    // SET INITIAL PASSWORD
    // =========================================================

    public async Task
        SetInitialPasswordAsync(
            int userId,
            SetInitialPasswordRequestDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(
                nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(
                dto.NewPassword))
        {
            throw new ArgumentException(
                "New password is required.");
        }

        if (dto.NewPassword !=
            dto.ConfirmNewPassword)
        {
            throw new ArgumentException(
                "Passwords do not match.");
        }

        if (dto.NewPassword.Length < 8)
        {
            throw new ArgumentException(
                "Password must be at least 8 characters long.");
        }

        // =====================================================
        // FIND USER
        // =====================================================

        var user =
            await _authRepository
                .GetUserByIdAsync(
                    userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        // =====================================================
        // OTP MUST BE VERIFIED
        // =====================================================

        if (!user.IsPhoneVerified)
        {
            throw new UnauthorizedAccessException(
                "Phone number has not been verified.");
        }

        // =====================================================
        // PASSWORD SETUP MUST STILL BE REQUIRED
        // =====================================================

        if (!user.MustChangePassword)
        {
            throw new InvalidOperationException(
                "Initial password has already been set.");
        }

        // =====================================================
        // HASH PERMANENT PASSWORD
        // =====================================================

        user.PasswordHash =
            _passwordHasher
                .HashPassword(
                    user,
                    dto.NewPassword);

        user.MustChangePassword =
            false;

        user.IsActive =
            true;

        await _authRepository
            .UpdateUserAsync(
                user);
    }


    // =========================================================
    // REFRESH TOKEN
    // Not required for current demo
    // =========================================================

    public Task<RefreshTokenResponseDto>
        RefreshTokenAsync(
            RefreshTokenRequestDto dto)
    {
        throw new NotSupportedException(
            "Refresh token persistence is not available until the shared authentication persistence is implemented.");
    }


    // =========================================================
    // CHANGE PASSWORD
    // =========================================================

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(
                nameof(dto));
        }

        var user =
            await _authRepository
                .GetUserByIdAsync(
                    userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var passwordResult =
            _passwordHasher
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.CurrentPassword);

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Current password is incorrect.");
        }

        if (string.IsNullOrWhiteSpace(
                dto.NewPassword))
        {
            throw new ArgumentException(
                "New password is required.");
        }

        if (dto.NewPassword.Length < 8)
        {
            throw new ArgumentException(
                "New password must be at least 8 characters long.");
        }

        user.PasswordHash =
            _passwordHasher
                .HashPassword(
                    user,
                    dto.NewPassword);

        user.MustChangePassword =
            false;

        await _authRepository
            .UpdateUserAsync(
                user);
    }


    // =========================================================
    // FORGOT PASSWORD
    // Later
    // =========================================================

    public Task ForgotPasswordAsync(
        ForgotPasswordRequestDto dto)
    {
        throw new NotSupportedException(
            "Password reset persistence is not available until the shared authentication persistence is implemented.");
    }


    // =========================================================
    // RESET PASSWORD
    // Later
    // =========================================================

    public Task ResetPasswordAsync(
        ResetPasswordRequestDto dto)
    {
        throw new NotSupportedException(
            "Password reset persistence is not available until the shared authentication persistence is implemented.");
    }


    // =========================================================
    // AUTH OTP VERIFY
    // Student registration uses StudentRegistration service.
    // =========================================================

    public async Task VerifyOtpAsync(
        VerifyOtpRequestDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(
                nameof(dto));
        }

        var user =
            await _authRepository
                .GetUserByUsernameAsync(
                    dto.Username);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        throw new NotSupportedException(
            "Use the Student Registration OTP verification flow for student onboarding.");
    }


    // =========================================================
    // AUTH RESEND OTP
    // Student registration uses StudentRegistration service.
    // =========================================================

    public async Task ResendOtpAsync(
        ResendOtpRequestDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(
                nameof(dto));
        }

        var user =
            await _authRepository
                .GetUserByUsernameAsync(
                    dto.Username);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        throw new NotSupportedException(
            "Use the Student Registration OTP resend flow for student onboarding.");
    }


    // =========================================================
    // LOGOUT
    // Later server-side token revocation
    // =========================================================

    public Task LogoutAsync(
        int userId)
    {
        throw new NotSupportedException(
            "Token revocation persistence is not available until the shared authentication persistence is implemented.");
    }


    // =========================================================
    // LOGOUT ALL
    // Later
    // =========================================================

    public Task LogoutAllAsync(
        int userId)
    {
        throw new NotSupportedException(
            "Token revocation persistence is not available until the shared authentication persistence is implemented.");
    }


    // =========================================================
    // GENERATE ACCESS TOKEN
    // =========================================================

    private string GenerateAccessToken(
        User user,
        DateTime expiresAt,
        string? roleName = null)
    {
        var key =
            _configuration[
                "Jwt:Key"];

        if (string.IsNullOrWhiteSpace(
                key))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured.");
        }

        var issuer =
            _configuration[
                "Jwt:Issuer"];

        var audience =
            _configuration[
                "Jwt:Audience"];

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString()),

                new(
                    ClaimTypes.Name,
                    user.Username),

                new(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString())
            };

        if (!string.IsNullOrWhiteSpace(
                roleName))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    roleName));
        }

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer:
                    issuer,

                audience:
                    audience,

                claims:
                    claims,

                expires:
                    expiresAt,

                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    // =========================================================
    // GENERATE REFRESH TOKEN
    // =========================================================

    private static string
        GenerateRefreshToken()
    {
        var randomBytes =
            RandomNumberGenerator
                .GetBytes(64);

        return Convert.ToBase64String(
            randomBytes);
    }


    // =========================================================
    // ACCESS TOKEN LIFETIME
    // =========================================================

    private int
        GetAccessTokenLifetimeMinutes()
    {
        var configuredValue =
            _configuration[
                "Jwt:AccessTokenLifetimeMinutes"];

        return int.TryParse(
            configuredValue,
            out var minutes)
            ? minutes
            : 60;
    }
}