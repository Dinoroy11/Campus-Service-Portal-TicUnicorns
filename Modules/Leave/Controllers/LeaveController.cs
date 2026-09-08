using CampusServicePortal.Modules.Leave.DTOs;
using CampusServicePortal.Modules.Leave.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Leave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveDto>>> GetAllLeaves()
        {
            var leaves = await _leaveService.GetAllLeavesAsync();

            return Ok(leaves);
        }

        [HttpGet("{leaveId}")]
        public async Task<ActionResult<LeaveDto>> GetLeaveById(int leaveId)
        {
            var leave = await _leaveService.GetLeaveByIdAsync(leaveId);

            if (leave == null)
            {
                return NotFound();
            }

            return Ok(leave);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveDto>> CreateLeave(
            CreateLeaveDto leaveDto)
        {
            var leave = await _leaveService.CreateLeaveAsync(leaveDto);

            return Ok(leave);
        }

        [HttpPut("{leaveId}")]
        public async Task<ActionResult<LeaveDto>> UpdateLeave(
            int leaveId,
            UpdateLeaveDto leaveDto)
        {
            var leave =
                await _leaveService.UpdateLeaveAsync(leaveId, leaveDto);

            if (leave == null)
            {
                return NotFound();
            }

            return Ok(leave);
        }

        [HttpDelete("{leaveId}")]
        public async Task<IActionResult> DeleteLeave(int leaveId)
        {
            var deleted =
                await _leaveService.DeleteLeaveAsync(leaveId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}