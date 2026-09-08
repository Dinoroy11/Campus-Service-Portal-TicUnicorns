using CampusServicePortal.Modules.Identity.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpPost]
    public async Task<ActionResult<AuditLogDto>> Create(
        [FromBody] AuditLogDto dto)
    {
        var result = await _auditLogService.CreateAsync(dto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
    {
        var result = await _auditLogService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByUserId(
        int userId)
    {
        var result = await _auditLogService.GetByUserIdAsync(userId);

        return Ok(result);
    }
}