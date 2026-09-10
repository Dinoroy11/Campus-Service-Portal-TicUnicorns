using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SmsTestController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SmsTestController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromQuery] string mobileNumber)
    {
        await _smsService.SendAsync(
            mobileNumber,
            "Campus Service Portal test SMS.");

        return Ok(new
        {
            message = "Test SMS sent successfully."
        });
    }
}