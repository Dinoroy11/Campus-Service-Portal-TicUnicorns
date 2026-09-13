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

    // Student: own profile only
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentDto>> GetMyProfile()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized(new { message = "Invalid user identity." });
        }

        var student = await _studentService.GetByUserIdAsync(userId);

        if (student is null)
        {
            return NotFound(new { message = "Student profile not found." });
        }

        return Ok(student);
    }

    // Admin: student management
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        return Ok(await _studentService.GetAllAsync());
    }

    [HttpGet("{studentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentDto>> GetById(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student is null)
        {
            return NotFound(new { message = "Student not found." });
        }

        return Ok(student);
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

            if (student is null)
            {
                return NotFound(new { message = "Student not found." });
            }

            return Ok(student);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
