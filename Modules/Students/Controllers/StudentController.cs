using System.Security.Claims;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        => Ok(await _studentService.GetAllAsync());

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentDto>> GetMy()
    {
        try
        {
            var student = await _studentService.GetByUserIdAsync(GetCurrentUserId());

            return student is null
                ? NotFound(new { message = "Student profile not found." })
                : Ok(student);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // Full own-profile update. Keep this for future profile editing screens.
    [HttpPut("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentDto>> UpdateMy(
        [FromBody] UpdateStudentDto dto)
    {
        try
        {
            var current = await _studentService.GetByUserIdAsync(GetCurrentUserId());

            if (current is null)
                return NotFound(new { message = "Student profile not found." });

            var updated = await _studentService.UpdateAsync(current.StudentId, dto);

            return updated is null
                ? NotFound(new { message = "Student profile not found." })
                : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // One-time hostel eligibility update.
    // This endpoint intentionally updates only Gender, so FirstName/LastName
    // validation from the full profile update does not block hostel setup.
    [HttpPatch("my/hostel-eligibility")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentDto>> UpdateMyHostelEligibility(
        [FromBody] UpdateHostelEligibilityDto dto)
    {
        try
        {
            var updated = await _studentService.UpdateHostelEligibilityAsync(
                GetCurrentUserId(),
                dto.Gender);

            return updated is null
                ? NotFound(new { message = "Student profile not found." })
                : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("{studentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentDto>> GetById(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        return student is null
            ? NotFound(new { message = "Student not found." })
            : Ok(student);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentDto>> Create(
        [FromBody] CreateStudentDto dto)
    {
        try
        {
            var student = await _studentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { studentId = student.StudentId },
                student);
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

    [HttpPut("{studentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentDto>> Update(
        int studentId,
        [FromBody] UpdateStudentDto dto)
    {
        try
        {
            var student = await _studentService.UpdateAsync(studentId, dto);

            return student is null
                ? NotFound(new { message = "Student not found." })
                : Ok(student);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
