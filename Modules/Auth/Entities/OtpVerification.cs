namespace CampusServicePortal.Modules.Auth.Entities;

public class OtpVerification
{
    public int OtpVerificationId { get; set; }

    public int UserId { get; set; }

    public string OtpHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public int AttemptCount { get; set; }

    public bool IsVerified { get; set; }
}
