using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Enums;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Services;

public class CertificatesService : ICertificatesService
{
    private readonly ICertificatesRepository _certificatesRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public CertificatesService(
        ICertificatesRepository certificatesRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _certificatesRepository = certificatesRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<CertificateResponseDto>> GetAllAsync()
    {
        var certificates = await _certificatesRepository.GetAllAsync();
        return certificates.Select(MapToResponseDto).ToList();
    }

    public async Task<CertificateResponseDto?> GetByIdAsync(int certificateId)
    {
        var certificate = await _certificatesRepository.GetByIdAsync(certificateId);
        return certificate == null ? null : MapToResponseDto(certificate);
    }

    public async Task<IReadOnlyList<CertificateResponseDto>> GetMyAsync(int userId)
    {
        var student = await GetActiveStudentByUserIdAsync(userId);
        var certificates = await _certificatesRepository
            .GetByStudentIdAsync(student.StudentId);

        return certificates.Select(MapToResponseDto).ToList();
    }

    public async Task<CertificateResponseDto?> GetMyByIdAsync(
        int userId,
        int certificateId)
    {
        var student = await GetActiveStudentByUserIdAsync(userId);
        var certificate = await _certificatesRepository.GetByIdAsync(certificateId);

        if (certificate == null || certificate.StudentId != student.StudentId)
            return null;

        return MapToResponseDto(certificate);
    }

    public async Task<CertificateResponseDto> CreateAsync(
        int userId,
        CreateCertificateDto dto)
    {
        var student = await GetActiveStudentByUserIdAsync(userId);
        var certificateType = NormalizeCertificateType(dto.CertificateType);
        var purpose = NormalizePurpose(dto.Purpose);

        var duplicatePending = await _certificatesRepository.HasPendingRequestAsync(
            student.StudentId,
            certificateType);

        if (duplicatePending)
        {
            throw new InvalidOperationException(
                $"You already have a pending {certificateType} certificate request.");
        }

        var certificate = new CertificatesEntities
        {
            StudentId = student.StudentId,
            CertificateType = certificateType,
            Purpose = purpose,
            Status = CertificateStatus.Pending.ToString(),
            RequestedAt = DateTime.UtcNow,
            ProcessedAt = null,
            RejectionReason = null,
            DocumentPath = null
        };

        var created = await _certificatesRepository.CreateAsync(certificate);
        return MapToResponseDto(created);
    }

    public async Task<CertificateResponseDto> UpdateMyAsync(
        int userId,
        int certificateId,
        UpdateCertificateDto dto)
    {
        var student = await GetActiveStudentByUserIdAsync(userId);
        var certificate = await _certificatesRepository.GetByIdAsync(certificateId)
            ?? throw new KeyNotFoundException("Certificate request not found.");

        if (certificate.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "This certificate request does not belong to the logged-in student.");
        }

        if (!certificate.Status.Equals(
                CertificateStatus.Pending.ToString(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only a pending certificate request can be edited.");
        }

        var certificateType = NormalizeCertificateType(dto.CertificateType);
        var purpose = NormalizePurpose(dto.Purpose);

        var duplicatePending = await _certificatesRepository.HasPendingRequestAsync(
            student.StudentId,
            certificateType,
            certificate.CertificateId);

        if (duplicatePending)
        {
            throw new InvalidOperationException(
                $"You already have another pending {certificateType} certificate request.");
        }

        certificate.CertificateType = certificateType;
        certificate.Purpose = purpose;

        await _certificatesRepository.UpdateAsync(certificate);
        return MapToResponseDto(certificate);
    }

    public async Task<CertificateResponseDto> UpdateStatusAsync(
        int certificateId,
        UpdateCertificateStatusDto dto)
    {
        var certificate = await _certificatesRepository.GetByIdAsync(certificateId)
            ?? throw new KeyNotFoundException("Certificate request not found.");

        var newStatus = NormalizeStatus(dto.Status);
        ValidateStatusTransition(certificate.Status, newStatus);

        if (newStatus == CertificateStatus.Rejected.ToString())
        {
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                throw new ArgumentException(
                    "Rejection reason is required when rejecting a certificate request.");
            }

            if (dto.RejectionReason.Trim().Length > 500)
                throw new ArgumentException("Rejection reason cannot exceed 500 characters.");

            certificate.RejectionReason = dto.RejectionReason.Trim();
            certificate.DocumentPath = null;
        }
        else
        {
            certificate.RejectionReason = null;
        }

        if (newStatus == CertificateStatus.Ready.ToString())
        {
            if (!string.IsNullOrWhiteSpace(dto.DocumentPath))
            {
                if (dto.DocumentPath.Trim().Length > 500)
                    throw new ArgumentException("Document path cannot exceed 500 characters.");

                certificate.DocumentPath = dto.DocumentPath.Trim();
            }
        }
        else if (newStatus == CertificateStatus.Approved.ToString())
        {
            certificate.DocumentPath = null;
        }

        certificate.Status = newStatus;
        certificate.ProcessedAt = DateTime.UtcNow;

        await _certificatesRepository.UpdateAsync(certificate);
        await SendStatusNotificationAsync(certificate);

        return MapToResponseDto(certificate);
    }

    private async Task<Student> GetActiveStudentByUserIdAsync(int userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException(
                "Student profile was not found for the logged-in user.");

        if (!student.IsActive)
            throw new UnauthorizedAccessException("Student account is not active.");

        return student;
    }

    private async Task SendStatusNotificationAsync(CertificatesEntities certificate)
    {
        var student = await _studentRepository.GetByIdAsync(certificate.StudentId);

        if (student?.UserId is not int userId)
            return;

        var title = certificate.Status switch
        {
            "Approved" => "Certificate Request Approved",
            "Rejected" => "Certificate Request Rejected",
            "Ready" => "Certificate Ready",
            _ => "Certificate Request Updated"
        };

        var message = certificate.Status switch
        {
            "Approved" =>
                $"Your {certificate.CertificateType} certificate request has been approved and is being prepared.",

            "Rejected" =>
                $"Your {certificate.CertificateType} certificate request was rejected. Reason: {certificate.RejectionReason}",

            "Ready" =>
                $"Your {certificate.CertificateType} certificate is ready.",

            _ =>
                $"Your {certificate.CertificateType} certificate request status is now {certificate.Status}."
        };

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId,
            Title = title,
            Message = message,
            ReferenceType = "CertificateRequest",
            ReferenceId = certificate.CertificateId
        });
    }

    private static string NormalizeCertificateType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Certificate type is required.");

        var compact = value
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        if (!Enum.TryParse<CertificateType>(compact, true, out var parsed))
        {
            throw new ArgumentException(
                "Invalid certificate type. Allowed values: Bonafide, Transcript, CompletionLetter.");
        }

        return parsed.ToString();
    }

    private static string NormalizePurpose(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Purpose is required.");

        var purpose = value.Trim();

        if (purpose.Length > 500)
            throw new ArgumentException("Purpose cannot exceed 500 characters.");

        return purpose;
    }

    private static string NormalizeStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Status is required.");

        var compact = value
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        if (!Enum.TryParse<CertificateStatus>(compact, true, out var parsed))
        {
            throw new ArgumentException(
                "Invalid certificate status. Allowed values: Pending, Approved, Rejected, Ready.");
        }

        return parsed.ToString();
    }

    private static void ValidateStatusTransition(
        string currentStatus,
        string newStatus)
    {
        if (currentStatus.Equals(newStatus, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Certificate request is already {newStatus}.");

        var allowed = currentStatus switch
        {
            "Pending" => newStatus is "Approved" or "Rejected",
            "Approved" => newStatus == "Ready",
            "Rejected" => false,
            "Ready" => false,
            _ => false
        };

        if (!allowed)
        {
            throw new InvalidOperationException(
                $"Invalid certificate status transition: {currentStatus} -> {newStatus}.");
        }
    }

    private static CertificateResponseDto MapToResponseDto(
        CertificatesEntities certificate)
    {
        return new CertificateResponseDto
        {
            CertificateId = certificate.CertificateId,
            StudentId = certificate.StudentId,
            CertificateType = certificate.CertificateType,
            Purpose = certificate.Purpose,
            Status = certificate.Status,
            RequestedAt = certificate.RequestedAt,
            ProcessedAt = certificate.ProcessedAt,
            RejectionReason = certificate.RejectionReason,
            DocumentPath = certificate.DocumentPath
        };
    }
}
