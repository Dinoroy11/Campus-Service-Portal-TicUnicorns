using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificatesService _certificatesService;

        public CertificatesController(
            ICertificatesService certificatesService)
        {
            _certificatesService = certificatesService;
        }

        // GET: api/Certificates
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var certificates =
                await _certificatesService.GetAllAsync();

            return Ok(certificates);
        }

        // GET: api/Certificates/1
        [HttpGet("{certificateId}")]
        public async Task<IActionResult> GetById(int certificateId)
        {
            var certificate =
                await _certificatesService.GetByIdAsync(certificateId);

            if (certificate == null)
                return NotFound("Certificate request not found.");

            return Ok(certificate);
        }

        // GET: api/Certificates/student/101
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var certificates =
                await _certificatesService.GetByStudentIdAsync(studentId);

            return Ok(certificates);
        }

        // POST: api/Certificates
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCertificateDto dto)
        {
            var certificate =
                await _certificatesService.CreateAsync(dto);

            return Ok(certificate);
        }

        // PUT: api/Certificates/1
        [HttpPut("{certificateId}")]
        public async Task<IActionResult> Update(
            int certificateId,
            [FromBody] UpdateCertificateDto dto)
        {
            var result =
                await _certificatesService.UpdateAsync(
                    certificateId,
                    dto);

            if (!result)
                return NotFound("Certificate request not found.");

            return Ok("Certificate request updated successfully.");
        }

        // PUT: api/Certificates/1/status
        [HttpPut("{certificateId}/status")]
        public async Task<IActionResult> UpdateStatus(
            int certificateId,
            [FromBody] UpdateCertificateStatusDto dto)
        {
            var result =
                await _certificatesService.UpdateStatusAsync(
                    certificateId,
                    dto);

            if (!result)
                return NotFound("Certificate request not found.");

            return Ok("Certificate status updated successfully.");
        }

        // PUT: api/Certificates/1/assign
        [HttpPut("{certificateId}/assign")]
        public async Task<IActionResult> Assign(
            int certificateId,
            [FromBody] AssignCertificateDto dto)
        {
            var result =
                await _certificatesService.AssignAsync(
                    certificateId,
                    dto);

            if (!result)
                return NotFound("Certificate request not found.");

            return Ok("Certificate assigned successfully.");
        }

        // DELETE: api/Certificates/1
        [HttpDelete("{certificateId}")]
        public async Task<IActionResult> Delete(int certificateId)
        {
            var result =
                await _certificatesService.DeleteAsync(certificateId);

            if (!result)
                return NotFound("Certificate request not found.");

            return Ok("Certificate request deleted successfully.");
        }
    }
}