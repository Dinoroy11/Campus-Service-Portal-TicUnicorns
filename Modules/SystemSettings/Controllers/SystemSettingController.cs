using CampusServicePortal.Modules.SystemSettings.DTOs;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.SystemSettings.Controllers;

[ApiController]
[Route("api/SystemSetting")]
[Authorize(Roles = "Admin")]
public class SystemSettingController : ControllerBase
{
    private readonly ISystemSettingService _service;

    public SystemSettingController(ISystemSettingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllSystemSettingsAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var setting = await _service.GetSystemSettingByIdAsync(id);
        return setting == null ? NotFound() : Ok(setting);
    }

    [HttpGet("key/{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await _service.GetSystemSettingByKeyAsync(key);
        return setting == null ? NotFound() : Ok(setting);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSystemSettingDto dto)
    {
        try
        {
            var setting = await _service.CreateSystemSettingAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = setting.SettingId },
                setting);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSystemSettingDto dto)
    {
        try
        {
            var setting = await _service.UpdateSystemSettingAsync(id, dto);
            return setting == null ? NotFound() : Ok(setting);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateSystemSettingStatusDto dto)
    {
        var setting = await _service.SetActiveStatusAsync(id, dto.IsActive);
        return setting == null ? NotFound() : Ok(setting);
    }

    // Kept for compatibility: DELETE now safely deactivates instead of hard-deleting.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var updated = await _service.DeactivateSystemSettingAsync(id);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("reservation-hold-minutes")]
    public async Task<IActionResult> GetReservationHoldMinutes()
    {
        var minutes = await _service.GetReservationHoldMinutesAsync();
        return Ok(new { minutes });
    }

    [HttpPut("reservation-hold-minutes")]
    public async Task<IActionResult> SetReservationHoldMinutes(
        UpdateReservationHoldMinutesDto dto)
    {
        try
        {
            var setting = await _service
                .SetReservationHoldMinutesAsync(dto.Minutes);

            return Ok(setting);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
