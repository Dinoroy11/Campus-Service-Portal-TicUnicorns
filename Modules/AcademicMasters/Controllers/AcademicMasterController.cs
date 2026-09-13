using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.DTOs;
using CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.AcademicMasters.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicMasterController : ControllerBase
{
    private readonly IAcademicMasterService _service;

    public AcademicMasterController(IAcademicMasterService service)
    {
        _service = service;
    }

    [HttpGet("universities")]
    public async Task<ActionResult<IReadOnlyList<UniversityDto>>> GetUniversities()
    {
        return Ok(await _service.GetUniversitiesAsync());
    }

    [HttpGet("universities/{universityId:int}/faculties")]
    public async Task<ActionResult<IReadOnlyList<FacultyDto>>> GetFaculties(
        int universityId)
    {
        return Ok(await _service.GetFacultiesByUniversityAsync(universityId));
    }

    [HttpGet("faculties/{facultyId:int}/departments")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartmentsByFaculty(
        int facultyId)
    {
        return Ok(await _service.GetDepartmentsByFacultyAsync(facultyId));
    }

    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments()
    {
        return Ok(await _service.GetDepartmentsAsync());
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IReadOnlyList<AcademicMasterTreeDto>>> GetTree()
    {
        return Ok(await _service.GetTreeAsync());
    }
}
