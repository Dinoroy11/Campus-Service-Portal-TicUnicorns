using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SportsEventDepartmentLimitController : ControllerBase
{
    private readonly ISportsEventDepartmentLimitService _departmentLimitService;

    public SportsEventDepartmentLimitController(
        ISportsEventDepartmentLimitService departmentLimitService)
    {
        _departmentLimitService = departmentLimitService;
    }

    [HttpGet("event/{sportsEventId:int}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetBySportsEventId(int sportsEventId)
    {
        try
        {
            return Ok(await _departmentLimitService
                .GetBySportsEventIdAsync(sportsEventId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] SportsEventDepartmentLimitDto dto)
    {
        try
        {
            var result = await _departmentLimitService.CreateAsync(dto);
            return Ok(result);
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

    [HttpPut("{sportsEventDepartmentLimitId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int sportsEventDepartmentLimitId,
        [FromBody] SportsEventDepartmentLimitDto dto)
    {
        try
        {
            var result = await _departmentLimitService.UpdateAsync(
                sportsEventDepartmentLimitId,
                dto);

            return result == null
                ? NotFound(new { message = "Department limit not found." })
                : Ok(result);
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
}
