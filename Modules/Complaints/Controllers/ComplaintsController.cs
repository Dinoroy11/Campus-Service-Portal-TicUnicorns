using CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Complaints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintsService _complaintsService;

        public ComplaintsController(IComplaintsService complaintsService)
        {
            _complaintsService = complaintsService;
        }

        // GET: api/Complaints
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var complaints = await _complaintsService.GetAllAsync();

            return Ok(complaints);
        }

        // GET: api/Complaints/5
        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetById(int complaintId)
        {
            var complaint =
                await _complaintsService.GetByIdAsync(complaintId);

            if (complaint == null)
                return NotFound("Complaint not found.");

            return Ok(complaint);
        }

        // GET: api/Complaints/student/5
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var complaints =
                await _complaintsService.GetByStudentIdAsync(studentId);

            return Ok(complaints);
        }

        // POST: api/Complaints
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateComplaintDto dto)
        {
            var complaint =
                await _complaintsService.CreateAsync(dto);

            return Ok(complaint);
        }

        // PUT: api/Complaints/5
        [HttpPut("{complaintId}")]
        public async Task<IActionResult> Update(
            int complaintId,
            [FromBody] UpdateComplaintDto dto)
        {
            var result =
                await _complaintsService.UpdateAsync(
                    complaintId,
                    dto);

            if (!result)
                return NotFound("Complaint not found.");

            return Ok("Complaint updated successfully.");
        }

        // PUT: api/Complaints/5/status
        [HttpPut("{complaintId}/status")]
        public async Task<IActionResult> UpdateStatus(
            int complaintId,
            [FromBody] UpdateComplaintStatusDto dto)
        {
            var result =
                await _complaintsService.UpdateStatusAsync(
                    complaintId,
                    dto);

            if (!result)
                return NotFound("Complaint not found.");

            return Ok("Complaint status updated successfully.");
        }

        // PUT: api/Complaints/5/assign
        [HttpPut("{complaintId}/assign")]
        public async Task<IActionResult> Assign(
            int complaintId,
            [FromBody] AssignComplaintDto dto)
        {
            var result =
                await _complaintsService.AssignAsync(
                    complaintId,
                    dto);

            if (!result)
                return NotFound("Complaint not found.");

            return Ok("Complaint assigned successfully.");
        }

        // DELETE: api/Complaints/5
        [HttpDelete("{complaintId}")]
        public async Task<IActionResult> Delete(int complaintId)
        {
            var result =
                await _complaintsService.DeleteAsync(complaintId);

            if (!result)
                return NotFound("Complaint not found.");

            return Ok("Complaint deleted successfully.");
        }
    }
}