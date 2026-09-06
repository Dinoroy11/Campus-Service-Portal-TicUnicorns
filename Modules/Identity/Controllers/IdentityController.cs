using CampusServicePortal.Modules.Identity.DTOs;
using CampusServicePortal.Modules.Identity.Interfaces.Service;

using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    // GET: api/Identity/users
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _identityService.GetUsersAsync();
        return Ok(users);
    }

    // GET: api/Identity/users/{userId}
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId)
    {
        var user = await _identityService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // GET: api/Identity/roles
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _identityService.GetRolesAsync();
        return Ok(roles);
    }

    // GET: api/Identity/roles/{roleId}
    [HttpGet("roles/{roleId}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(int roleId)
    {
        var role = await _identityService.GetRoleByIdAsync(roleId);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    // POST: api/Identity/roles
    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole(
        [FromBody] CreateRoleDto dto)
    {
        var role = await _identityService.CreateRoleAsync(dto);

        return CreatedAtAction(
            nameof(GetRoleById),
            new { roleId = role.RoleId },
            role);
    }

    // PUT: api/Identity/roles/{roleId}
    [HttpPut("roles/{roleId}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(
        int roleId,
        [FromBody] UpdateRoleDto dto)
    {
        var role = await _identityService.UpdateRoleAsync(roleId, dto);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    // GET: api/Identity/permissions
    [HttpGet("permissions")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
    {
        var permissions = await _identityService.GetPermissionsAsync();
        return Ok(permissions);
    }

    // POST: api/Identity/users/{userId}/roles/{roleId}
    [HttpPost("users/{userId}/roles/{roleId}")]
    public async Task<ActionResult<UserRoleDto>> AssignRoleToUser(
        int userId,
        int roleId)
    {
        var result = await _identityService.AssignRoleToUserAsync(
            userId,
            roleId);

        return Ok(result);
    }

    // POST: api/Identity/roles/{roleId}/permissions/{permissionId}
    [HttpPost("roles/{roleId}/permissions/{permissionId}")]
    public async Task<ActionResult<RolePermissionDto>> AssignPermissionToRole(
        int roleId,
        int permissionId)
    {
        var result = await _identityService.AssignPermissionToRoleAsync(
            roleId,
            permissionId);

        return Ok(result);
    }

    // POST: api/Identity/staff-assignments
    [HttpPost("staff-assignments")]
    public async Task<ActionResult<StaffAssignmentDto>> AssignStaffScope(
        [FromBody] StaffAssignmentDto dto)
    {
        var result = await _identityService.AssignStaffScopeAsync(dto);

        return Ok(result);
    }

    // GET: api/Identity/audit-logs
    [HttpGet("audit-logs")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs()
    {
        var auditLogs = await _identityService.GetAuditLogsAsync();
        return Ok(auditLogs);
    }
}