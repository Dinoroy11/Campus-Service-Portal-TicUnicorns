using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanteenController : ControllerBase
    {
        private readonly ICanteenService _service;

        public CanteenController(ICanteenService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{canteenId}")]
        public async Task<IActionResult> GetById(int canteenId)
        {
            var result = await _service.GetByIdAsync(canteenId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var result = await _service.GetByStudentIdAsync(studentId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCanteenDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { canteenId = result.CanteenId },
                result);
        }

        [HttpPut("{canteenId}")]
        public async Task<IActionResult> Update(
            int canteenId,
            UpdateCanteenDto dto)
        {
            var result = await _service.UpdateAsync(
                canteenId,
                dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{canteenId}/status")]
        public async Task<IActionResult> UpdateStatus(
            int canteenId,
            UpdateCanteenStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(
                canteenId,
                dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{canteenId}")]
        public async Task<IActionResult> Delete(int canteenId)
        {
            var deleted = await _service.DeleteAsync(canteenId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
