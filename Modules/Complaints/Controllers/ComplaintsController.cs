using System.Security.Claims;
using CampusServicePortal.Modules.Complaints.DTOs;
using CampusServicePortal.Modules.Complaints.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Complaints.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintsService _complaintsService;

    public ComplaintsController(IComplaintsService complaintsService)
    {
        _complaintsService = complaintsService;
    }

    // =========================================================
    // SHARED: CATEGORIES
    // =========================================================

    [HttpGet("categories")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetCategories()
    {
        var categories =
            await _complaintsService.GetAllCategoriesAsync();

        // Students only need categories they can currently use.
        if (User.IsInRole("Student"))
            categories = categories.Where(x => x.IsActive).ToList();

        return Ok(categories);
    }

    // =========================================================
    // ADMIN: CATEGORY MANAGEMENT
    // =========================================================

    [HttpGet("categories/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category =
            await _complaintsService.GetCategoryByIdAsync(id);

        return category == null
            ? NotFound(new { message = "Complaint category not found." })
            : Ok(category);
    }

    [HttpPost("categories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateComplaintCategoryDto dto)
    {
        try
        {
            var category =
                await _complaintsService.CreateCategoryAsync(dto);

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.ComplaintCategoryId },
                category);
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

    [HttpPut("categories/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] CreateComplaintCategoryDto dto)
    {
        try
        {
            await _complaintsService.UpdateCategoryAsync(id, dto);
            return Ok(new { message = "Complaint category updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    // =========================================================
    // STUDENT: OWN COMPLAINTS
    // =========================================================

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyComplaints()
    {
        try
        {
            return Ok(await _complaintsService
                .GetMyComplaintsAsync(GetCurrentUserId()));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my/{id:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyComplaint(int id)
    {
        try
        {
            var complaint = await _complaintsService
                .GetMyComplaintByIdAsync(GetCurrentUserId(), id);

            return complaint == null
                ? NotFound(new { message = "Complaint not found." })
                : Ok(complaint);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateComplaint(
        [FromBody] CreateComplaintDto dto)
    {
        try
        {
            var complaint = await _complaintsService
                .CreateComplaintAsync(GetCurrentUserId(), dto);

            return Ok(complaint);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/confirm-resolution")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ConfirmResolution(
        int id,
        [FromBody] ConfirmComplaintResolutionDto dto)
    {
        try
        {
            return Ok(await _complaintsService
                .ConfirmResolutionAsync(GetCurrentUserId(), id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my/{id:int}/history")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyComplaintHistory(int id)
    {
        try
        {
            return Ok(await _complaintsService
                .GetMyComplaintStatusHistoryAsync(
                    GetCurrentUserId(), id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // =========================================================
    // ADMIN: COMPLAINT WORKFLOW
    // =========================================================

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllComplaints()
    {
        return Ok(await _complaintsService.GetAllComplaintsAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetComplaint(int id)
    {
        var complaint =
            await _complaintsService.GetComplaintByIdAsync(id);

        return complaint == null
            ? NotFound(new { message = "Complaint not found." })
            : Ok(complaint);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateComplaintStatus(
        int id,
        [FromBody] UpdateComplaintDto dto)
    {
        try
        {
            return Ok(await _complaintsService
                .UpdateComplaintStatusAsync(
                    id,
                    GetCurrentUserId(),
                    dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/history")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetComplaintHistory(int id)
    {
        try
        {
            return Ok(await _complaintsService
                .GetComplaintStatusHistoryAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
