using CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryService _laundryService;

        public LaundryController(ILaundryService laundryService)
        {
            _laundryService = laundryService;
        }

        // GET: api/Laundry
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var laundryRequests =
                await _laundryService.GetAllAsync();

            return Ok(laundryRequests);
        }

        // GET: api/Laundry/1
        [HttpGet("{laundryId}")]
        public async Task<IActionResult> GetById(int laundryId)
        {
            var laundry =
                await _laundryService.GetByIdAsync(laundryId);

            if (laundry == null)
                return NotFound("Laundry request not found.");

            return Ok(laundry);
        }

        // GET: api/Laundry/student/101
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var laundryRequests =
                await _laundryService.GetByStudentIdAsync(studentId);

            return Ok(laundryRequests);
        }

        // POST: api/Laundry
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateLaundryDto dto)
        {
            var laundry =
                await _laundryService.CreateAsync(dto);

            return Ok(laundry);
        }

        // PUT: api/Laundry/1
        [HttpPut("{laundryId}")]
        public async Task<IActionResult> Update(
            int laundryId,
            [FromBody] UpdateLaundryDto dto)
        {
            var result =
                await _laundryService.UpdateAsync(
                    laundryId,
                    dto);

            if (!result)
                return NotFound("Laundry request not found.");

            return Ok("Laundry request updated successfully.");
        }

        // PUT: api/Laundry/1/status
        [HttpPut("{laundryId}/status")]
        public async Task<IActionResult> UpdateStatus(
            int laundryId,
            [FromBody] UpdateLaundryStatusDto dto)
        {
            var result =
                await _laundryService.UpdateStatusAsync(
                    laundryId,
                    dto);

            if (!result)
                return NotFound("Laundry request not found.");

            return Ok("Laundry status updated successfully.");
        }

        // PUT: api/Laundry/1/assign
        [HttpPut("{laundryId}/assign")]
        public async Task<IActionResult> Assign(
            int laundryId,
            [FromBody] AssignLaundryDto dto)
        {
            var result =
                await _laundryService.AssignAsync(
                    laundryId,
                    dto);

            if (!result)
                return NotFound("Laundry request not found.");

            return Ok("Laundry request assigned successfully.");
        }

        // DELETE: api/Laundry/1
        [HttpDelete("{laundryId}")]
        public async Task<IActionResult> Delete(int laundryId)
        {
            var result =
                await _laundryService.DeleteAsync(laundryId);

            if (!result)
                return NotFound("Laundry request not found.");

            return Ok("Laundry request deleted successfully.");
        }
    }
}