using System.Security.Claims;
using CampusServicePortal.Modules.Leave.DTOs;
using CampusServicePortal.Modules.Leave.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Leave.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    // =========================================================
    // LEAVE TYPES
    // =========================================================

    [HttpGet("types")]
    public async Task<IActionResult> GetLeaveTypes(
        [FromQuery] bool includeInactive = false)
    {
        // Only Admin can request inactive types.
        var showInactive = includeInactive && User.IsInRole("Admin");
        return Ok(await _leaveService.GetLeaveTypesAsync(showInactive));
    }

    [HttpPost("types")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLeaveType(
        [FromBody] CreateLeaveTypeDto dto)
    {
        try
        {
            return Ok(await _leaveService.CreateLeaveTypeAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("types/{leaveTypeId:int}/active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetLeaveTypeStatus(
        int leaveTypeId,
        [FromBody] UpdateLeaveTypeStatusDto dto)
    {
        var result = await _leaveService
            .SetLeaveTypeStatusAsync(leaveTypeId, dto.IsActive);

        return result is null
            ? NotFound(new { message = "Leave type not found." })
            : Ok(result);
    }

    // =========================================================
    // STUDENT FLOW
    // =========================================================

    [HttpPost("requests")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateLeave(
        [FromBody] CreateLeaveDto dto)
    {
        try
        {
            var result = await _leaveService
                .CreateLeaveAsync(GetCurrentUserId(), dto);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("requests/my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyLeaves()
    {
        try
        {
            return Ok(await _leaveService
                .GetMyLeavesAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("requests/my/{leaveRequestId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyLeave(int leaveRequestId)
    {
        try
        {
            var result = await _leaveService
                .GetMyLeaveByIdAsync(GetCurrentUserId(), leaveRequestId);

            return result is null
                ? NotFound(new { message = "Leave request not found." })
                : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // =========================================================
    // DEPARTMENT STAFF / ADMIN FLOW
    // DepartmentStaffAssignment controls scope.
    // Admin can view/review all requests as a temporary/full-access role.
    // =========================================================

    [HttpGet("requests/staff")]
    public async Task<IActionResult> GetStaffDashboard()
    {
        try
        {
            return Ok(await _leaveService.GetStaffDashboardAsync(
                GetCurrentUserId(),
                User.IsInRole("Admin")));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("requests/staff/{leaveRequestId:int}")]
    public async Task<IActionResult> GetStaffLeave(int leaveRequestId)
    {
        try
        {
            var result = await _leaveService.GetStaffLeaveByIdAsync(
                GetCurrentUserId(),
                User.IsInRole("Admin"),
                leaveRequestId);

            return result is null
                ? NotFound(new { message = "Leave request not found." })
                : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPatch("requests/{leaveRequestId:int}/decision")]
    public async Task<IActionResult> DecideLeave(
        int leaveRequestId,
        [FromBody] UpdateLeaveDto dto)
    {
        try
        {
            var result = await _leaveService.DecideLeaveAsync(
                GetCurrentUserId(),
                User.IsInRole("Admin"),
                leaveRequestId,
                dto);

            return result is null
                ? NotFound(new { message = "Leave request not found." })
                : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID claim is missing.");

        return userId;
    }
}
