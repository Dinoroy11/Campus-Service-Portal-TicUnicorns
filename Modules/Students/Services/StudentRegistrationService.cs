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

    public async Task SendOtpAsync(
        StudentRegistrationDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(
                dto.UniversityStudentId))
        {
            throw new ArgumentException(
                "University Student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
                dto.MobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.");
        }

        // =====================================================
        // 1. FIND MASTER STUDENT
        // =====================================================

        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    dto.UniversityStudentId);

        if (masterStudent is null)
        {
            throw new KeyNotFoundException(
                "Student was not found in the master student list.");
        }

        // =====================================================
        // 2. MASTER STUDENT MUST BE ACTIVE
        // =====================================================

        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Student is not active.");
        }

        // =====================================================
        // 3. MOBILE NUMBER MUST MATCH MASTER LIST
        // =====================================================

        if (!string.Equals(
                masterStudent.MobileNumber,
                dto.MobileNumber,
                StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");
        }

        // =====================================================
        // 4. FIND EXISTING USER
        // =====================================================

        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        // =====================================================
        // 5. CHECK WHETHER STUDENT PROFILE EXISTS
        // =====================================================

        var studentProfileExists =
            await _studentRepository
                .ExistsByMasterStudentIdAsync(
                    masterStudent.MasterStudentId);

        // =====================================================
        // 6. COMPLETED STUDENT MUST NOT REGISTER AGAIN
        //
        // IMPORTANT:
        // If profile exists BUT MustChangePassword = true,
        // onboarding is still incomplete.
        // We allow OTP resend so the student does not get stuck.
        // =====================================================

        if (studentProfileExists &&
            user is not null &&
            !user.MustChangePassword)
        {
            throw new InvalidOperationException(
                "Student is already registered.");
        }

        // =====================================================
        // 7. CREATE USER IF FIRST ATTEMPT
        // =====================================================

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

                // Temporary unusable value.
                // Permanent password will be created
                // after OTP verification.
                PasswordHash =
                    Guid.NewGuid().ToString(),

                CreatedAt =
                    DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }
        else
        {
            // If permanent password was already created,
            // registration is already complete.
            if (!user.MustChangePassword)
            {
                throw new InvalidOperationException(
                    "Student is already registered.");
            }

            // This is an incomplete first-login attempt.
            // Allow a fresh OTP.
            user.PhoneNumber =
                masterStudent.MobileNumber;

            // Fresh OTP must be verified again.
            user.IsPhoneVerified = false;

            user.MustChangePassword = true;

            await _userRepository.UpdateAsync(user);
        }

        // =====================================================
        // 8. GENERATE OTP
        // =====================================================

        var otp =
            await _otpService.GenerateAsync(
                user.UserId);

        // =====================================================
        // 9. SEND OTP
        // =====================================================

        await _smsService.SendAsync(
            masterStudent.MobileNumber,
            $"Your Campus Service Portal OTP is {otp}. It is valid for 5 minutes.");
    }


    // =========================================================
    // VERIFY OTP
    // =========================================================

    public async Task<PasswordSetupResponseDto> VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp)
    {
        if (string.IsNullOrWhiteSpace(
                universityStudentId))
        {
            throw new ArgumentException(
                "University Student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
                mobileNumber))
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
        // 1. FIND MASTER STUDENT
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
        // 2. MASTER STUDENT MUST BE ACTIVE
        // =====================================================

        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Student is not active.");
        }

        // =====================================================
        // 3. MOBILE MUST MATCH MASTER LIST
        // =====================================================

        if (!string.Equals(
                masterStudent.MobileNumber,
                mobileNumber,
                StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");
        }

        // =====================================================
        // 4. FIND USER
        // =====================================================

        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Student account was not found. Send OTP first.");
        }

        // =====================================================
        // 5. CHECK EXISTING STUDENT PROFILE
        // =====================================================

        var studentProfileExists =
            await _studentRepository
                .ExistsByMasterStudentIdAsync(
                    masterStudent.MasterStudentId);

        // If password setup already completed,
        // student registration is fully complete.
        if (studentProfileExists &&
            !user.MustChangePassword)
        {
            throw new InvalidOperationException(
                "Student is already registered.");
        }

        // =====================================================
        // 6. VERIFY OTP
        // =====================================================

        await _otpService.VerifyAsync(
            user.UserId,
            otp);

        // =====================================================
        // 7. ACTIVATE USER
        // =====================================================

        user.PhoneNumber =
            masterStudent.MobileNumber;

        user.IsPhoneVerified = true;

        user.IsActive = true;

        // Still needs permanent password.
        user.MustChangePassword = true;

        await _userRepository.UpdateAsync(user);

        // =====================================================
        // 8. CREATE STUDENT PROFILE ONLY ONCE
        // =====================================================

        if (!studentProfileExists)
        {
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
        }

        // =====================================================
        // 9. GET STUDENT ROLE
        // =====================================================

        var studentRole =
            await GetStudentRoleAsync();

        // =====================================================
        // 10. ASSIGN STUDENT ROLE ONLY ONCE
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

            await _userRoleRepository
                .AddAsync(userRole);
        }

        // =====================================================
        // 11. CREATE PASSWORD SETUP TOKEN
        // =====================================================

        var setupToken =
            await _authService
                .CreatePasswordSetupTokenAsync(
                    user);

        // =====================================================
        // 12. RETURN TOKEN
        // =====================================================

        return new PasswordSetupResponseDto
        {
            SetupToken =
                setupToken,

            ExpiresAt =
                DateTime.UtcNow.AddMinutes(10),

            Username =
                user.Username
        };
    }


    // =========================================================
    // GET STUDENT ROLE
    // =========================================================

    private async Task<Role> GetStudentRoleAsync()
    {
        var roles =
            await _roleRepository
                .GetAllAsync();

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
        SplitStudentName(
            string fullName)
    {
        if (string.IsNullOrWhiteSpace(
                fullName))
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