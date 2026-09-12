using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;

public interface ICertificatesService
{
    Task<IReadOnlyList<CertificateResponseDto>> GetAllAsync();

    Task<CertificateResponseDto?> GetByIdAsync(int certificateId);

    Task<IReadOnlyList<CertificateResponseDto>> GetMyAsync(int userId);

    Task<CertificateResponseDto?> GetMyByIdAsync(
        int userId,
        int certificateId);

    Task<CertificateResponseDto> CreateAsync(
        int userId,
        CreateCertificateDto dto);

    Task<CertificateResponseDto> UpdateMyAsync(
        int userId,
        int certificateId,
        UpdateCertificateDto dto);

    Task<CertificateResponseDto> UpdateStatusAsync(
        int certificateId,
        UpdateCertificateStatusDto dto);
}
