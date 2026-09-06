using CampusServicePortal.Modules.Complaints.DTOs;
using CampusServicePortal.Modules.Complaints.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Complaints.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintsService _complaintsService;

    public ComplaintsController(IComplaintsService complaintsService)
    {
        _complaintsService = complaintsService;
    }

    // =========================================================
    // Categories
    // =========================================================

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories =
            await _complaintsService.GetAllCategoriesAsync();

        return Ok(categories);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category =
            await _complaintsService.GetCategoryByIdAsync(id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        CreateComplaintCategoryDto dto)
    {
        var category =
            await _complaintsService.CreateCategoryAsync(dto);

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.ComplaintCategoryId },
            category);
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        CreateComplaintCategoryDto dto)
    {
        try
        {
            await _complaintsService.UpdateCategoryAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // =========================================================
    // Complaints
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAllComplaints()
    {
        var complaints =
            await _complaintsService.GetAllComplaintsAsync();

        return Ok(complaints);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComplaint(int id)
    {
        var complaint =
            await _complaintsService.GetComplaintByIdAsync(id);

        if (complaint == null)
            return NotFound();

        return Ok(complaint);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetComplaintsByStudent(
        int studentId)
    {
        var complaints =
            await _complaintsService
                .GetComplaintsByStudentIdAsync(studentId);

        return Ok(complaints);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComplaint(
        CreateComplaintDto dto)
    {
        try
        {
            var complaint =
                await _complaintsService
                    .CreateComplaintAsync(dto);

            return CreatedAtAction(
                nameof(GetComplaint),
                new { id = complaint.ComplaintId },
                complaint);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComplaint(
        int id,
        UpdateComplaintDto dto)
    {
        try
        {
            await _complaintsService
                .UpdateComplaintAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // =========================================================
    // Status History
    // =========================================================

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetComplaintHistory(int id)
    {
        try
        {
            var history =
                await _complaintsService
                    .GetComplaintStatusHistoryAsync(id);

            return Ok(history);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}