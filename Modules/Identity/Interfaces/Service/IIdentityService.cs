using CampusServicePortal.Modules.Identity.DTOs;

namespace CampusServicePortal.Modules.Identity.Interfaces.Service;

public interface IIdentityService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();

    Task<UserDto?> GetUserByIdAsync(int userId);

    Task<IEnumerable<RoleDto>> GetRolesAsync();

    Task<RoleDto?> GetRoleByIdAsync(int roleId);

    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);

    Task<RoleDto?> UpdateRoleAsync(int roleId, UpdateRoleDto dto);

    Task<IEnumerable<PermissionDto>> GetPermissionsAsync();

    Task<UserRoleDto> AssignRoleToUserAsync(int userId, int roleId);

    Task<RolePermissionDto> AssignPermissionToRoleAsync(
        int roleId,
        int permissionId);

    Task<StaffAssignmentDto> AssignStaffScopeAsync(
        StaffAssignmentDto dto);

    Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync();
}