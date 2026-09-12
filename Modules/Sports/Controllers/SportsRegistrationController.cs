using System.Security.Claims;
using CampusServicePortal_TicUnicorns.Modules.Sports.DTOs;
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

    // ADMIN
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _registrationService.GetAllAsync());
    }

    [HttpGet("{sportsRegistrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int sportsRegistrationId)
    {
        try
        {
            var registration =
                await _registrationService.GetByIdAsync(sportsRegistrationId);

            return registration == null
                ? NotFound(new { message = "Sports registration not found." })
                : Ok(registration);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("event/{sportsEventId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetBySportsEventId(int sportsEventId)
    {
        try
        {
            return Ok(await _registrationService
                .GetBySportsEventIdAsync(sportsEventId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{sportsRegistrationId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int sportsRegistrationId,
        [FromBody] UpdateSportsRegistrationStatusDto dto)
    {
        try
        {
            var registration = await _registrationService.UpdateStatusAsync(
                sportsRegistrationId,
                dto.Status);

            return registration == null
                ? NotFound(new { message = "Sports registration not found." })
                : Ok(registration);
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

    // STUDENT
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMy()
    {
        try
        {
            return Ok(await _registrationService.GetMyAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateMy(
        [FromBody] CreateSportsRegistrationDto dto)
    {
        try
        {
            var registration = await _registrationService.CreateMyAsync(
                GetCurrentUserId(),
                dto);

            return Ok(registration);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
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

    [HttpPatch("my/{sportsRegistrationId:int}/cancel")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CancelMy(int sportsRegistrationId)
    {
        try
        {
            var registration = await _registrationService.CancelMyAsync(
                GetCurrentUserId(),
                sportsRegistrationId);

            return registration == null
                ? NotFound(new { message = "Sports registration not found." })
                : Ok(registration);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
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

    private int GetCurrentUserId()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
