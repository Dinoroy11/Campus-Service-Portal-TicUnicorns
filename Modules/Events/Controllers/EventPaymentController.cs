using System.Security.Claims;
using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventPaymentController : ControllerBase
{
    private readonly IEventPaymentService _paymentService;
    private readonly IStudentRepository _studentRepository;

    public EventPaymentController(
        IEventPaymentService paymentService,
        IStudentRepository studentRepository)
    {
        _paymentService = paymentService;
        _studentRepository = studentRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<EventPaymentDto>>> GetAll()
    {
        return Ok(await _paymentService.GetAllAsync());
    }

    [HttpGet("{eventPaymentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventPaymentDto>> GetById(int eventPaymentId)
    {
        var payment = await _paymentService.GetByIdAsync(eventPaymentId);
        return payment == null ? NotFound("Event payment not found.") : Ok(payment);
    }

    [HttpGet("registration/{registrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventPaymentDto>> GetByRegistrationId(int registrationId)
    {
        var payment = await _paymentService.GetByRegistrationIdAsync(registrationId);
        return payment == null
            ? NotFound("Payment not found for this registration.")
            : Ok(payment);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EventPaymentDto>> Create([FromBody] EventPaymentDto dto)
    {
        try
        {
            var student = await GetCurrentStudentAsync();
            return Ok(await _paymentService.CreateAsync(dto, student.StudentId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventPaymentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int eventPaymentId,
        [FromBody] EventPaymentDto dto)
    {
        try
        {
            var updated = await _paymentService.UpdateAsync(eventPaymentId, dto);
            return updated ? NoContent() : NotFound("Event payment not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<CampusServicePortal_TicUnicorns.Modules.Students.Entities.Student>
        GetCurrentStudentAsync()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return await _studentRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Student profile was not found for the logged-in user.");
    }
}
