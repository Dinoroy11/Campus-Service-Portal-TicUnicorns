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
    private readonly ISportsEventDepartmentLimitService
        _departmentLimitService;

    public SportsEventDepartmentLimitController(
        ISportsEventDepartmentLimitService departmentLimitService)
    {
        _departmentLimitService = departmentLimitService;
    }

    [HttpGet("event/{sportsEventId:int}")]
    public async Task<
        ActionResult<IEnumerable<SportsEventDepartmentLimitDto>>>
        GetBySportsEventId(int sportsEventId)
    {
        var limits =
            await _departmentLimitService
                .GetBySportsEventIdAsync(sportsEventId);

        return Ok(limits);
    }

    [HttpPost]
    public async Task<
        ActionResult<SportsEventDepartmentLimitDto>>
        Create(SportsEventDepartmentLimitDto dto)
    {
        var departmentLimit =
            await _departmentLimitService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetBySportsEventId),
            new
            {
                sportsEventId =
                    departmentLimit.SportsEventId
            },
            departmentLimit);
    }

    [HttpPut("{sportsEventDepartmentLimitId:int}")]
    public async Task<
        ActionResult<SportsEventDepartmentLimitDto>>
        Update(
            int sportsEventDepartmentLimitId,
            SportsEventDepartmentLimitDto dto)
    {
        var departmentLimit =
            await _departmentLimitService.UpdateAsync(
                sportsEventDepartmentLimitId,
                dto);

        if (departmentLimit == null)
            return NotFound();

        return Ok(departmentLimit);
    }
}