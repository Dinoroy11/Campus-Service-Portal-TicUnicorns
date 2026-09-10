using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using System.Security.Cryptography;
using System.Text;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Services;

public class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;

    private const int OtpLength = 6;
    private const int OtpExpiryMinutes = 5;
    private const int MaxAttempts = 5;

    public OtpService(IOtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<OtpVerification> GenerateAsync(int userId)
    {
        var otp = GenerateOtp();

        var otpVerification = new OtpVerification
        {
            UserId = userId,
            OtpHash = HashOtp(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 0,
            IsVerified = false,
            VerifiedAt = null
        };

        await _otpRepository.AddAsync(otpVerification);

        // TODO: Send OTP to user's mobile number.
        // SMS provider integration will be added separately.

        return otpVerification;
    }

    public async Task VerifyAsync(int userId, string otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            throw new ArgumentException("OTP is required.");
        }

        var otpVerification =
            await _otpRepository.GetLatestAsync(userId);

        if (otpVerification is null)
        {
            throw new KeyNotFoundException(
                "No OTP verification request was found.");
        }

        if (otpVerification.IsVerified)
        {
            throw new InvalidOperationException(
                "OTP has already been verified.");
        }

        if (otpVerification.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "OTP has expired.");
        }

        if (otpVerification.AttemptCount >= MaxAttempts)
        {
            throw new InvalidOperationException(
                "Maximum OTP verification attempts exceeded.");
        }

        otpVerification.AttemptCount++;

        var otpHash = HashOtp(otp);

        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(otpVerification.OtpHash),
                Convert.FromBase64String(otpHash)))
        {
            await _otpRepository.UpdateAsync(otpVerification);

            throw new UnauthorizedAccessException(
                "Invalid OTP.");
        }

        otpVerification.IsVerified = true;
        otpVerification.VerifiedAt = DateTime.UtcNow;

        await _otpRepository.UpdateAsync(otpVerification);
    }

    public async Task ResendAsync(int userId)
    {
        var otp = GenerateOtp();

        var otpVerification = new OtpVerification
        {
            UserId = userId,
            OtpHash = HashOtp(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 0,
            IsVerified = false,
            VerifiedAt = null
        };

        await _otpRepository.AddAsync(otpVerification);

        // TODO: Send new OTP to user's mobile number.
    }

    private static string GenerateOtp()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1_000_000);

        return value.ToString("D6");
    }

    private static string HashOtp(string otp)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(otp));

        return Convert.ToBase64String(hash);
    }
}