using System.Security.Claims;
using CampusServicePortal.Modules.Gym.DTOs;
using CampusServicePortal.Modules.Gym.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Gym.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GymController : ControllerBase
{
    private readonly IGymService _gymService;

    public GymController(IGymService gymService)
    {
        _gymService = gymService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GymDto>>> GetAllGyms()
    {
        return Ok(await _gymService.GetAllGymsAsync());
    }

    [HttpGet("{gymId:int}")]
    public async Task<ActionResult<GymDto>> GetGymById(int gymId)
    {
        var gym = await _gymService.GetGymByIdAsync(gymId);
        return gym == null ? NotFound() : Ok(gym);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GymDto>> CreateGym(
        [FromBody] CreateGymDto gymDto)
    {
        return Ok(await _gymService.CreateGymAsync(gymDto));
    }

    [HttpPut("{gymId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GymDto>> UpdateGym(
        int gymId,
        [FromBody] CreateGymDto gymDto)
    {
        var result = await _gymService.UpdateGymAsync(gymId, gymDto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{gymId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGym(int gymId)
    {
        return await _gymService.DeleteGymAsync(gymId)
            ? NoContent()
            : NotFound();
    }

    [HttpPost("slots")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GymSlotDto>> CreateSlot(
        [FromBody] CreateGymSlotDto dto)
    {
        return Ok(await _gymService.CreateSlotAsync(dto));
    }

    [HttpGet("{gymId:int}/availability")]
    public async Task<ActionResult<GymSlotAvailabilityDto>> GetAvailability(
        int gymId,
        [FromQuery] DateTime date)
    {
        return Ok(await _gymService.GetAvailabilityAsync(gymId, date));
    }

    [HttpPost("bookings")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<GymBookingDto>> CreateBooking(
        [FromBody] CreateGymBookingRequestDto dto)
    {
        return Ok(await _gymService.CreateBookingAsync(
            GetCurrentUserId(),
            dto));
    }

    [HttpGet("bookings")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<GymBookingDto>>> GetAllBookings()
    {
        return Ok(await _gymService.GetAllBookingsAsync());
    }

    [HttpGet("bookings/my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IEnumerable<GymBookingDto>>> GetMyBookings()
    {
        return Ok(await _gymService.GetMyBookingsAsync(GetCurrentUserId()));
    }

    [HttpPost("bookings/{bookingId:int}/pay")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<GymBookingDto>> PayBooking(
        int bookingId,
        [FromBody] GymPaymentDto dto)
    {
        return Ok(await _gymService.PayBookingAsync(
            GetCurrentUserId(),
            bookingId,
            dto));
    }

    [HttpDelete("bookings/{bookingId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<GymBookingDto>> CancelBooking(int bookingId)
    {
        return Ok(await _gymService.CancelBookingAsync(
            GetCurrentUserId(),
            bookingId));
    }

    [HttpPost("bookings/{bookingId:int}/complete")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GymBookingDto>> CompleteBooking(int bookingId)
    {
        return Ok(await _gymService.CompleteBookingAsync(bookingId));
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}
