using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service
{
    public interface ICertificatesService
    {
        Task<IEnumerable<CertificateResponseDto>> GetAllAsync();

        Task<CertificateResponseDto?> GetByIdAsync(int certificateId);

        Task<IEnumerable<CertificateResponseDto>> GetByStudentIdAsync(
            int studentId);

        Task<CertificateResponseDto> CreateAsync(
            CreateCertificateDto dto);

        Task<bool> UpdateAsync(
            int certificateId,
            UpdateCertificateDto dto);

        Task<bool> UpdateStatusAsync(
            int certificateId,
            UpdateCertificateStatusDto dto);

        Task<bool> DeleteAsync(int certificateId);
    }
}