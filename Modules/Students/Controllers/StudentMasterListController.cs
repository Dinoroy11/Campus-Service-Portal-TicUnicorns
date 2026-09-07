using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentMasterListController : ControllerBase
{
    private readonly IStudentMasterListService
        _studentMasterListService;

    public StudentMasterListController(
        IStudentMasterListService studentMasterListService)
    {
        _studentMasterListService = studentMasterListService;
    }

    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<StudentMasterListDto>>> GetAll()
    {
        var masterStudents =
            await _studentMasterListService.GetAllAsync();

        return Ok(masterStudents);
    }

    [HttpGet("{masterStudentId:int}")]
    public async Task<ActionResult<StudentMasterListDto>> GetById(
        int masterStudentId)
    {
        var masterStudent =
            await _studentMasterListService
                .GetByIdAsync(masterStudentId);

        if (masterStudent is null)
        {
            return NotFound(new
            {
                message = "Master student not found."
            });
        }

        return Ok(masterStudent);
    }

    [HttpGet("university-student/{universityStudentId}")]
    public async Task<ActionResult<StudentMasterListDto>>
        GetByUniversityStudentId(
            string universityStudentId)
    {
        var masterStudent =
            await _studentMasterListService
                .GetByUniversityStudentIdAsync(
                    universityStudentId);

        if (masterStudent is null)
        {
            return NotFound(new
            {
                message = "Master student not found."
            });
        }

        return Ok(masterStudent);
    }

    [HttpPost]
    public async Task<ActionResult<StudentMasterListDto>> Create(
        [FromBody] CreateStudentMasterListDto dto)
    {
        var masterStudent =
            await _studentMasterListService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                masterStudentId =
                    masterStudent.MasterStudentId
            },
            masterStudent);
    }

    [HttpPut("{masterStudentId:int}")]
    public async Task<ActionResult<StudentMasterListDto>> Update(
        int masterStudentId,
        [FromBody] UpdateStudentMasterListDto dto)
    {
        var masterStudent =
            await _studentMasterListService.UpdateAsync(
                masterStudentId,
                dto);

        if (masterStudent is null)
        {
            return NotFound(new
            {
                message = "Master student not found."
            });
        }

        return Ok(masterStudent);
    }
}