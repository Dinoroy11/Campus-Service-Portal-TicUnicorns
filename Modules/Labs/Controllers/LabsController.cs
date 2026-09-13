using System.Security.Claims;
using CampusServicePortal.Modules.Labs.DTOs;
using CampusServicePortal.Modules.Labs.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Labs.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabsController : ControllerBase
{
    private readonly ILabService _labService;
    private readonly IStudentRepository _studentRepository;

    public LabsController(
        ILabService labService,
        IStudentRepository studentRepository)
    {
        _labService = labService;
        _studentRepository = studentRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<List<LabDto>>> GetAllLabs()
    {
        return Ok(await _labService.GetAllLabsAsync());
    }

    [HttpGet("{labId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<LabDto>> GetLab(int labId)
    {
        var lab = await _labService.GetLabByIdAsync(labId);

        return lab == null
            ? NotFound(new { message = "Lab not found." })
            : Ok(lab);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LabDto>> CreateLab([FromBody] CreateLabDto dto)
    {
        try
        {
            var lab = await _labService.CreateLabAsync(dto);
            return CreatedAtAction(nameof(GetLab), new { labId = lab.LabId }, lab);
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

    [HttpPut("{labId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLab(int labId, [FromBody] UpdateLabDto dto)
    {
        try
        {
            await _labService.UpdateLabAsync(labId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("{labId:int}/seats")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<List<LabSeatDto>>> GetSeats(int labId)
    {
        try
        {
            return Ok(await _labService.GetSeatsByLabIdAsync(labId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("seats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LabSeatDto>> CreateSeat([FromBody] CreateLabSeatDto dto)
    {
        try
        {
            return Ok(await _labService.CreateSeatAsync(dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpPatch("seats/{labSeatId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LabSeatStatusUpdateResultDto>> UpdateSeatStatus(
        int labSeatId,
        [FromBody] UpdateLabSeatStatusDto dto)
    {
        try
        {
            return Ok(await _labService.UpdateSeatStatusAsync(labSeatId, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("{labId:int}/timeslots")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<List<LabTimeSlotDto>>> GetTimeSlots(int labId)
    {
        try
        {
            return Ok(await _labService.GetTimeSlotsByLabIdAsync(labId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("timeslots")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LabTimeSlotDto>> CreateTimeSlot(
        [FromBody] CreateLabTimeSlotDto dto)
    {
        try
        {
            return Ok(await _labService.CreateTimeSlotAsync(dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("{labId:int}/availability")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<LabAvailabilityDto>> GetAvailability(
        int labId,
        [FromQuery] int timeSlotId,
        [FromQuery] DateTime bookingDate,
        [FromQuery] TimeSpan? requestedStartTime = null,
        [FromQuery] double? requestedHours = null)
    {
        try
        {
            return Ok(await _labService.GetAvailabilityAsync(
                labId,
                timeSlotId,
                bookingDate,
                requestedStartTime,
                requestedHours));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpPost("bookings")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<LabBookingDto>> CreateBooking(
        [FromBody] CreateLabBookingDto dto)
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            dto.StudentId = student.StudentId;

            return Ok(await _labService.CreateBookingAsync(dto));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("bookings/my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<LabBookingDto>>> GetMyBookings()
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            return Ok(await _labService.GetBookingsByStudentIdAsync(student.StudentId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("bookings")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<LabBookingDto>>> GetAllBookings()
    {
        return Ok(await _labService.GetAllBookingsAsync());
    }

    [HttpGet("bookings/{labBookingId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<LabBookingDto>> GetBooking(int labBookingId)
    {
        var booking = await _labService.GetBookingByIdAsync(labBookingId);

        if (booking == null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        if (User.IsInRole("Student"))
        {
            var student = await GetCurrentStudentAsync();
            if (booking.StudentId != student.StudentId)
            {
                return Forbid();
            }
        }

        return Ok(booking);
    }

    [HttpPatch("bookings/{labBookingId:int}/cancel")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> CancelBooking(int labBookingId)
    {
        try
        {
            var booking = await _labService.GetBookingByIdAsync(labBookingId);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found." });
            }

            if (User.IsInRole("Student"))
            {
                var student = await GetCurrentStudentAsync();
                if (booking.StudentId != student.StudentId)
                {
                    return Forbid();
                }
            }

            await _labService.CancelBookingAsync(labBookingId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private async Task<CampusServicePortal_TicUnicorns.Modules.Students.Entities.Student>
        GetCurrentStudentAsync()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException(
                "Student profile was not found for the logged-in user.");
    }
}
