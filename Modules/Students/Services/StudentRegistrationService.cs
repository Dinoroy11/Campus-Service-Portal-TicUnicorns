using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;

using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Services;

public class StudentRegistrationService : IStudentRegistrationService
{
    private readonly IStudentMasterListRepository _masterListRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IAuthService _authService;

    public StudentRegistrationService(
        IStudentMasterListRepository masterListRepository,
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IOtpService otpService,
        ISmsService smsService,
        IAuthService authService)
    {
        _masterListRepository = masterListRepository;
        _userRepository = userRepository;
        _studentRepository = studentRepository;

        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;

        _otpService = otpService;
        _smsService = smsService;
        _authService = authService;
    }

    // =========================================================
    // SEND OTP
    // =========================================================

    public async Task SendOtpAsync(StudentRegistrationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UniversityStudentId))
        {
            throw new ArgumentException(
                "University Student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.MobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.");
        }

        // 1. Find student in Master Student List
        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    dto.UniversityStudentId);

        if (masterStudent is null)
        {
            throw new KeyNotFoundException(
                "Student was not found in the master student list.");
        }

        // 2. Check student is active
        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Student is not active.");
        }

        // 3. Verify mobile number
        if (masterStudent.MobileNumber != dto.MobileNumber)
        {
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");
        }

        // 4. Check whether Student profile already exists
        if (await _studentRepository
            .ExistsByMasterStudentIdAsync(
                masterStudent.MasterStudentId))
        {
            throw new InvalidOperationException(
                "Student is already registered.");
        }

        // 5. Find existing User
        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        // 6. Create User if it does not exist
        if (user is null)
        {
            user = new User
            {
                Username =
                    masterStudent.UniversityStudentId,

                PhoneNumber =
                    masterStudent.MobileNumber,

                IsPhoneVerified = false,

                IsActive = false,

                MustChangePassword = true,

                // Temporary password value.
                // Permanent password will be set
                // after first login.
                PasswordHash =
                    Guid.NewGuid().ToString(),

                CreatedAt =
                    DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }
        else
        {
            // Existing user should not already be verified
            if (user.IsPhoneVerified)
            {
                throw new InvalidOperationException(
                    "Student phone number is already verified.");
            }

            // Keep registered master-list number
            user.PhoneNumber =
                masterStudent.MobileNumber;

            await _userRepository.UpdateAsync(user);
        }

        // 7. Generate OTP
        var otp =
            await _otpService.GenerateAsync(
                user.UserId);

        // 8. Send OTP through Notify.lk
        await _smsService.SendAsync(
            masterStudent.MobileNumber,
            $"Your Campus Service Portal OTP is {otp}. It is valid for 5 minutes.");
    }


    // =========================================================
    // VERIFY OTP + FIRST LOGIN
    // =========================================================

    public async Task<LoginResponseDto> VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp)
    {
        if (string.IsNullOrWhiteSpace(universityStudentId))
        {
            throw new ArgumentException(
                "University Student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.");
        }

        if (string.IsNullOrWhiteSpace(otp))
        {
            throw new ArgumentException(
                "OTP is required.");
        }

        // =====================================================
        // 1. Find Student in Master Student List
        // =====================================================

        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    universityStudentId);

        if (masterStudent is null)
        {
            throw new KeyNotFoundException(
                "Student was not found in the master student list.");
        }

        // =====================================================
        // 2. Check Master Student is Active
        // =====================================================

        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Student is not active.");
        }

        // =====================================================
        // 3. Verify Mobile Number
        // =====================================================

        if (masterStudent.MobileNumber != mobileNumber)
        {
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");
        }

        // =====================================================
        // 4. Check Student Profile
        // =====================================================

        if (await _studentRepository
            .ExistsByMasterStudentIdAsync(
                masterStudent.MasterStudentId))
        {
            throw new InvalidOperationException(
                "Student is already registered.");
        }

        // =====================================================
        // 5. Find User
        // =====================================================

        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Student account was not found.");
        }

        // =====================================================
        // 6. Verify OTP
        // =====================================================

        await _otpService.VerifyAsync(
            user.UserId,
            otp);

        // =====================================================
        // 7. Activate User
        // =====================================================

        user.IsPhoneVerified = true;
        user.IsActive = true;

        // Important:
        // Student must set a permanent password
        // after first login.
        user.MustChangePassword = true;

        await _userRepository.UpdateAsync(user);

        // =====================================================
        // 8. Create Student Profile
        // =====================================================

        var (firstName, lastName) =
            SplitStudentName(
                masterStudent.StudentName);

        var student = new Student
        {
            MasterStudentId =
                masterStudent.MasterStudentId,

            UserId =
                user.UserId,

            FirstName =
                firstName,

            LastName =
                lastName,

            PhoneNumber =
                masterStudent.MobileNumber,

            Email =
                user.Email,

            DateOfBirth = null,

            Gender = null,

            AdmissionDate =
                DateTime.UtcNow,

            IsActive = true,

            CreatedAt =
                DateTime.UtcNow
        };

        await _studentRepository.AddAsync(
            student);

        // =====================================================
        // 9. Find Student Role
        // =====================================================

        var studentRole =
            await GetStudentRoleAsync();

        // =====================================================
        // 10. Assign Student Role
        // =====================================================

        var alreadyAssigned =
            await _userRoleRepository
                .ExistsAsync(
                    user.UserId,
                    studentRole.RoleId);

        if (!alreadyAssigned)
        {
            var userRole = new UserRole
            {
                UserId =
                    user.UserId,

                RoleId =
                    studentRole.RoleId,

                AssignedAt =
                    DateTime.UtcNow
            };

            await _userRoleRepository.AddAsync(
                userRole);
        }

        // =====================================================
        // 11. CREATE JWT
        // =====================================================

        return await _authService
            .CreateLoginResponseAsync(
                user,
                "Student");
    }


    // =========================================================
    // GET STUDENT ROLE
    // =========================================================

    private async Task<Role> GetStudentRoleAsync()
    {
        var roles =
            await _roleRepository.GetAllAsync();

        var studentRole =
            roles.FirstOrDefault(
                x => x.RoleName.Equals(
                    "Student",
                    StringComparison.OrdinalIgnoreCase));

        if (studentRole is null)
        {
            throw new InvalidOperationException(
                "Student role is not configured in the system.");
        }

        if (!studentRole.IsActive)
        {
            throw new InvalidOperationException(
                "Student role is inactive.");
        }

        return studentRole;
    }


    // =========================================================
    // SPLIT STUDENT NAME
    // =========================================================

    private static (
        string FirstName,
        string LastName)
        SplitStudentName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (
                "Student",
                string.Empty);
        }

        var parts =
            fullName
                .Trim()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
        {
            return (
                parts[0],
                string.Empty);
        }

        var firstName =
            parts[0];

        var lastName =
            string.Join(
                " ",
                parts.Skip(1));

        return (
            firstName,
            lastName);
    }
}