using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Sports.Enums;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Sports.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SportsRegistrationController : ControllerBase
{
    private readonly ISportsRegistrationService _registrationService;

    public SportsRegistrationController(
        ISportsRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SportsRegistrationDto>>>
        GetAll()
    {
        var registrations =
            await _registrationService.GetAllAsync();

        return Ok(registrations);
    }

    [HttpGet("{sportsRegistrationId:int}")]
    public async Task<ActionResult<SportsRegistrationDto>> GetById(
        int sportsRegistrationId)
    {
        var registration =
            await _registrationService
                .GetByIdAsync(sportsRegistrationId);

        if (registration == null)
            return NotFound();

        return Ok(registration);
    }

    [HttpGet("event/{sportsEventId:int}")]
    public async Task<ActionResult<IEnumerable<SportsRegistrationDto>>>
        GetBySportsEventId(int sportsEventId)
    {
        var registrations =
            await _registrationService
                .GetBySportsEventIdAsync(sportsEventId);

        return Ok(registrations);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<IEnumerable<SportsRegistrationDto>>>
        GetByStudentId(int studentId)
    {
        var registrations =
            await _registrationService
                .GetByStudentIdAsync(studentId);

        return Ok(registrations);
    }

    [HttpPost]
    public async Task<ActionResult<SportsRegistrationDto>> Create(
        CreateSportsRegistrationDto dto)
    {
        var registration =
            await _registrationService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                sportsRegistrationId =
                    registration.SportsRegistrationId
            },
            registration);
    }

    [HttpPatch("{sportsRegistrationId:int}/status")]
    public async Task<ActionResult<SportsRegistrationDto>> UpdateStatus(
        int sportsRegistrationId,
        SportsRegistrationStatus status)
    {
        var registration =
            await _registrationService.UpdateStatusAsync(
                sportsRegistrationId,
                status);

        if (registration == null)
            return NotFound();

        return Ok(registration);
    }
}
