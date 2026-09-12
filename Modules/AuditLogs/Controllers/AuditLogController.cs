using CampusServicePortal.Modules.Identity.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // Audit logs are created internally by backend services only.
    // There is intentionally NO public POST endpoint.

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
    {
        var result = await _auditLogService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByUserId(
        int userId)
    {
        var result = await _auditLogService.GetByUserIdAsync(userId);
        return Ok(result);
    }
}
