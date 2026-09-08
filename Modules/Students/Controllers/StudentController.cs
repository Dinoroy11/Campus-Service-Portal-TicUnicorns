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
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        var students = await _studentService.GetAllAsync();

        return Ok(students);
    }

    [HttpGet("{studentId:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student is null)
        {
            return NotFound(new
            {
                message = "Student not found."
            });
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(
        [FromBody] CreateStudentDto dto)
    {
        var student = await _studentService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { studentId = student.StudentId },
            student);
    }

    [HttpPut("{studentId:int}")]
    public async Task<ActionResult<StudentDto>> Update(
        int studentId,
        [FromBody] UpdateStudentDto dto)
    {
        var student = await _studentService.UpdateAsync(
            studentId,
            dto);

        if (student is null)
        {
            return NotFound(new
            {
                message = "Student not found."
            });
        }

        return Ok(student);
    }
}