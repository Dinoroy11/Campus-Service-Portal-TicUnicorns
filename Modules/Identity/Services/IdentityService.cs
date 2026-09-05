using CampusServicePortal.Modules.Identity.DTOs;
using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal.Modules.Identity.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public IdentityService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsPhoneVerified = user.IsPhoneVerified,
            MustChangePassword = user.MustChangePassword,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsPhoneVerified = user.IsPhoneVerified,
            MustChangePassword = user.MustChangePassword,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(role => new RoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        });
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role == null)
        {
            return null;
        }

        return new RoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        };
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName))
        {
            throw new ArgumentException("Role name is required.");
        }

        var roleExists = await _roleRepository.ExistsByNameAsync(dto.RoleName);

        if (roleExists)
        {
            throw new InvalidOperationException(
                "A role with the same name already exists.");
        }

        var role = new Role
        {
            RoleName = dto.RoleName.Trim(),
            Description = dto.Description,
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _roleRepository.AddAsync(role);

        return new RoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        };
    }

    public async Task<RoleDto?> UpdateRoleAsync(
        int roleId,
        UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(dto.RoleName))
        {
            throw new ArgumentException("Role name is required.");
        }

        if (!string.Equals(
                role.RoleName,
                dto.RoleName,
                StringComparison.OrdinalIgnoreCase))
        {
            var roleExists =
                await _roleRepository.ExistsByNameAsync(dto.RoleName);

            if (roleExists)
            {
                throw new InvalidOperationException(
                    "A role with the same name already exists.");
            }
        }

        role.RoleName = dto.RoleName.Trim();
        role.Description = dto.Description;
        role.IsActive = dto.IsActive;

        await _roleRepository.UpdateAsync(role);

        return new RoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        };
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        return permissions.Select(permission => new PermissionDto
        {
            PermissionId = permission.PermissionId,
            Code = permission.Code,
            Name = permission.Name,
            Module = permission.Module,
            Description = permission.Description,
            IsActive = permission.IsActive
        });
    }

    public async Task<UserRoleDto> AssignRoleToUserAsync(
        int userId,
        int roleId)
    {
        var userExists = await _userRepository.ExistsAsync(userId);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with ID {userId} was not found.");
        }

        var roleExists = await _roleRepository.ExistsAsync(roleId);

        if (!roleExists)
        {
            throw new KeyNotFoundException(
                $"Role with ID {roleId} was not found.");
        }

        // Persistence will be handled when the shared
        // repository / DbContext implementation is available.
        return new UserRoleDto
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };
    }

    public async Task<RolePermissionDto> AssignPermissionToRoleAsync(
        int roleId,
        int permissionId)
    {
        var roleExists = await _roleRepository.ExistsAsync(roleId);

        if (!roleExists)
        {
            throw new KeyNotFoundException(
                $"Role with ID {roleId} was not found.");
        }

        var permissionExists =
            await _permissionRepository.ExistsAsync(permissionId);

        if (!permissionExists)
        {
            throw new KeyNotFoundException(
                $"Permission with ID {permissionId} was not found.");
        }

        // Persistence will be handled when the shared
        // repository / DbContext implementation is available.
        return new RolePermissionDto
        {
            RoleId = roleId,
            PermissionId = permissionId
        };
    }

    public async Task<StaffAssignmentDto> AssignStaffScopeAsync(
        StaffAssignmentDto dto)
    {
        var userExists = await _userRepository.ExistsAsync(dto.UserId);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with ID {dto.UserId} was not found.");
        }

        var scopeCount = 0;

        if (dto.DepartmentId.HasValue)
        {
            scopeCount++;
        }

        if (dto.HostelId.HasValue)
        {
            scopeCount++;
        }

        if (dto.CanteenId.HasValue)
        {
            scopeCount++;
        }

        if (scopeCount == 0)
        {
            throw new ArgumentException(
                "At least one staff scope must be provided.");
        }

        if (scopeCount > 1)
        {
            throw new ArgumentException(
                "Only one staff scope can be assigned at a time.");
        }

        // Persistence will be handled when the shared
        // repository / DbContext implementation is available.
        return dto;
    }

    public Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync()
    {
        // AuditLog repository is not part of the current
        // repository interfaces. It will be connected when
        // the shared data layer is available.

        return Task.FromResult(
            Enumerable.Empty<AuditLogDto>());
    }
}