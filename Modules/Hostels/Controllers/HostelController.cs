using System.Security.Claims;
using CampusServicePortal.Modules.Hostels.DTOs;
using CampusServicePortal.Modules.Hostels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Hostels.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HostelController : ControllerBase
{
    private readonly IHostelService _hostelService;

    public HostelController(IHostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // =========================================================
    // SHARED: ADMIN + STUDENT
    // =========================================================

    [HttpGet]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetHostels()
    {
        var hostels = await _hostelService.GetHostelsAsync();
        return Ok(hostels);
    }

    [HttpGet("{hostelId}/blueprint")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetHostelBlueprint(int hostelId)
    {
        var hostel = await _hostelService
            .GetHostelBlueprintAsync(hostelId);

        if (hostel is null)
        {
            return NotFound(new { message = "Hostel not found." });
        }

        return Ok(hostel);
    }

    // =========================================================
    // ADMIN: HOSTEL STRUCTURE
    // =========================================================

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHostel(
        [FromBody] CreateHostelDto dto)
    {
        var result = await _hostelService.CreateHostelAsync(dto);
        return Ok(result);
    }

    [HttpPost("floors")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFloor(
        [FromBody] CreateFloorDto dto)
    {
        await _hostelService.CreateFloorAsync(dto);
        return Ok(new { message = "Floor created successfully." });
    }

    [HttpPost("rooms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoom(
        [FromBody] CreateRoomDto dto)
    {
        await _hostelService.CreateRoomAsync(dto);
        return Ok(new { message = "Room created successfully." });
    }

    [HttpPost("beds")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoomBed(
        [FromBody] CreateRoomBedDto dto)
    {
        await _hostelService.CreateRoomBedAsync(dto);
        return Ok(new { message = "Bed created successfully." });
    }

    [HttpPut("{hostelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHostel(
        int hostelId,
        [FromBody] UpdateHostelDto dto)
    {
        var updated = await _hostelService
            .UpdateHostelAsync(hostelId, dto);

        if (!updated)
        {
            return NotFound(new { message = "Hostel not found." });
        }

        return Ok(new { message = "Hostel updated successfully." });
    }

    [HttpPut("floors/{floorId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFloor(
        int floorId,
        [FromBody] UpdateFloorDto dto)
    {
        var updated = await _hostelService
            .UpdateFloorAsync(floorId, dto);

        if (!updated)
        {
            return NotFound(new { message = "Floor not found." });
        }

        return Ok(new { message = "Floor updated successfully." });
    }

    [HttpPut("rooms/{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(
        int roomId,
        [FromBody] UpdateRoomDto dto)
    {
        var updated = await _hostelService
            .UpdateRoomAsync(roomId, dto);

        if (!updated)
        {
            return NotFound(new { message = "Room not found." });
        }

        return Ok(new { message = "Room updated successfully." });
    }

    [HttpPut("beds/{bedId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoomBed(
        int bedId,
        [FromBody] UpdateRoomBedDto dto)
    {
        var updated = await _hostelService
            .UpdateRoomBedAsync(bedId, dto);

        if (!updated)
        {
            return NotFound(new { message = "Bed not found." });
        }

        return Ok(new { message = "Bed updated successfully." });
    }

    // =========================================================
    // STUDENT: 15-MINUTE BED HOLD
    // =========================================================

    [HttpPost("holds/{bedId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateHold(int bedId)
    {
        var result = await _hostelService
            .CreateHoldAsync(GetCurrentUserId(), bedId);

        return Ok(result);
    }

    [HttpDelete("holds/{holdId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ReleaseHold(int holdId)
    {
        var released = await _hostelService
            .ReleaseHoldAsync(GetCurrentUserId(), holdId);

        if (!released)
        {
            return NotFound(new { message = "Active hold not found." });
        }

        return Ok(new { message = "Hostel bed hold released." });
    }

    // =========================================================
    // STUDENT: APPLICATION
    // =========================================================

    [HttpPost("applications")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateApplication(
        [FromBody] CreateHostelApplicationDto dto)
    {
        var result = await _hostelService
            .CreateApplicationAsync(GetCurrentUserId(), dto);

        return Ok(result);
    }

    [HttpGet("applications/my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyApplications()
    {
        var result = await _hostelService
            .GetMyApplicationsAsync(GetCurrentUserId());

        return Ok(result);
    }

    [HttpDelete("applications/{applicationId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CancelApplication(int applicationId)
    {
        var cancelled = await _hostelService
            .CancelApplicationAsync(GetCurrentUserId(), applicationId);

        if (!cancelled)
        {
            return NotFound(new { message = "Application not found." });
        }

        return Ok(new { message = "Hostel application cancelled." });
    }

    // =========================================================
    // ADMIN: APPLICATION REVIEW
    // =========================================================

    [HttpGet("applications")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetApplications()
    {
        var result = await _hostelService.GetApplicationsAsync();
        return Ok(result);
    }

    [HttpGet("applications/{applicationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetApplication(int applicationId)
    {
        var result = await _hostelService
            .GetApplicationAsync(applicationId);

        if (result is null)
        {
            return NotFound(new { message = "Application not found." });
        }

        return Ok(result);
    }

    [HttpPatch("applications/{applicationId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateApplicationStatus(
        int applicationId,
        [FromBody] UpdateHostelApplicationStatusDto dto)
    {
        var updated = await _hostelService
            .UpdateApplicationStatusAsync(applicationId, dto);

        if (!updated)
        {
            return NotFound(new { message = "Application not found." });
        }

        return Ok(new { message = "Application status updated." });
    }

    // =========================================================
    // ADMIN: APPROVE + ALLOCATE
    // =========================================================

    [HttpPost("applications/{applicationId}/allocate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllocateApplication(
        int applicationId,
        [FromBody] AllocateHostelApplicationDto dto)
    {
        var result = await _hostelService
            .AllocateApplicationAsync(applicationId, dto);

        return Ok(result);
    }

    [HttpGet("allocations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllocations()
    {
        var result = await _hostelService.GetAllocationsAsync();
        return Ok(result);
    }

    [HttpGet("allocations/my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyAllocations()
    {
        var result = await _hostelService
            .GetMyAllocationsAsync(GetCurrentUserId());

        return Ok(result);
    }

    [HttpPost("allocations/{allocationId}/end")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EndAllocation(int allocationId)
    {
        var ended = await _hostelService.EndAllocationAsync(allocationId);

        if (!ended)
        {
            return NotFound(new { message = "Allocation not found." });
        }

        return Ok(new { message = "Hostel allocation ended." });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}
