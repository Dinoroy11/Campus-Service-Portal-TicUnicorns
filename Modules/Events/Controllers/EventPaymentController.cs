using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventPaymentController : ControllerBase
{
    private readonly IEventPaymentService _paymentService;

    public EventPaymentController(IEventPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventPaymentDto>>> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();

        return Ok(payments);
    }

    [HttpGet("{eventPaymentId:int}")]
    public async Task<ActionResult<EventPaymentDto>> GetById(
        int eventPaymentId)
    {
        var payment =
            await _paymentService.GetByIdAsync(eventPaymentId);

        if (payment == null)
            return NotFound("Event payment not found.");

        return Ok(payment);
    }

    [HttpGet("registration/{registrationId:int}")]
    public async Task<ActionResult<EventPaymentDto>> GetByRegistrationId(
        int registrationId)
    {
        var payment =
            await _paymentService.GetByRegistrationIdAsync(
                registrationId);

        if (payment == null)
            return NotFound(
                "Payment not found for this registration.");

        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<EventPaymentDto>> Create(
        [FromBody] EventPaymentDto dto)
    {
        try
        {
            var payment =
                await _paymentService.CreateAsync(dto);

            return Ok(payment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{eventPaymentId:int}")]
    public async Task<IActionResult> Update(
        int eventPaymentId,
        [FromBody] EventPaymentDto dto)
    {
        try
        {
            var updated =
                await _paymentService.UpdateAsync(
                    eventPaymentId,
                    dto);

            if (!updated)
                return NotFound("Event payment not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}