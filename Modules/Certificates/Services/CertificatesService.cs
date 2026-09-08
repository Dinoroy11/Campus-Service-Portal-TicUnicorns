using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Services
{
    public class CertificatesService : ICertificatesService
    {
        private readonly ICertificatesRepository _certificatesRepository;

        public CertificatesService(
            ICertificatesRepository certificatesRepository)
        {
            _certificatesRepository = certificatesRepository;
        }

        public async Task<IEnumerable<CertificateResponseDto>> GetAllAsync()
        {
            var certificates =
                await _certificatesRepository.GetAllAsync();

            return certificates.Select(MapToResponseDto);
        }

        public async Task<CertificateResponseDto?> GetByIdAsync(
            int certificateId)
        {
            var certificate =
                await _certificatesRepository.GetByIdAsync(certificateId);

            if (certificate == null)
                return null;

            return MapToResponseDto(certificate);
        }

        public async Task<IEnumerable<CertificateResponseDto>>
            GetByStudentIdAsync(int studentId)
        {
            var certificates =
                await _certificatesRepository.GetByStudentIdAsync(studentId);

            return certificates.Select(MapToResponseDto);
        }

        public async Task<CertificateResponseDto> CreateAsync(
            CreateCertificateDto dto)
        {
            var certificate = new CertificatesEntities
            {
                StudentId = dto.StudentId,
                CertificateType = dto.CertificateType,
                Purpose = dto.Purpose,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            var createdCertificate =
                await _certificatesRepository.CreateAsync(certificate);

            return MapToResponseDto(createdCertificate);
        }

        public async Task<bool> UpdateAsync(
            int certificateId,
            UpdateCertificateDto dto)
        {
            var certificate =
                await _certificatesRepository.GetByIdAsync(certificateId);

            if (certificate == null)
                return false;

            certificate.CertificateType = dto.CertificateType;
            certificate.Purpose = dto.Purpose;

            return await _certificatesRepository.UpdateAsync(certificate);
        }

        public async Task<bool> UpdateStatusAsync(
            int certificateId,
            UpdateCertificateStatusDto dto)
        {
            var certificate =
                await _certificatesRepository.GetByIdAsync(certificateId);

            if (certificate == null)
                return false;

            return await _certificatesRepository.UpdateStatusAsync(
                certificateId,
                dto.Status,
                dto.RejectionReason);
        }

        public async Task<bool> DeleteAsync(int certificateId)
        {
            var certificate =
                await _certificatesRepository.GetByIdAsync(certificateId);

            if (certificate == null)
                return false;

            return await _certificatesRepository.DeleteAsync(certificateId);
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
}