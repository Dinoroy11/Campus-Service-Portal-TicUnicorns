using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Services;

public class StudentRegistrationService : IStudentRegistrationService
{
    private readonly IStudentMasterListRepository _masterListRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;

    public StudentRegistrationService(
        IStudentMasterListRepository masterListRepository,
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IOtpService otpService,
        ISmsService smsService)
    {
        _masterListRepository = masterListRepository;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _otpService = otpService;
        _smsService = smsService;
    }

    public async Task SendOtpAsync(StudentRegistrationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UniversityStudentId))
            throw new ArgumentException(
                "University Student ID is required.");

        if (string.IsNullOrWhiteSpace(dto.MobileNumber))
            throw new ArgumentException(
                "Mobile number is required.");

        // 1. Find student in Master Student List
        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    dto.UniversityStudentId);

        if (masterStudent is null)
            throw new KeyNotFoundException(
                "Student was not found in the master student list.");

        // 2. Check master student is active
        if (!masterStudent.IsActive)
            throw new InvalidOperationException(
                "Student is not active.");

        // 3. Verify mobile number
        if (masterStudent.MobileNumber != dto.MobileNumber)
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");

        // 4. Check whether Student profile already exists
        if (await _studentRepository
            .ExistsByMasterStudentIdAsync(
                masterStudent.MasterStudentId))
        {
            throw new InvalidOperationException(
                "Student is already registered.");
        }

        // 5. Find existing User using University Student ID
        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        // 6. Create User if it does not exist
        if (user is null)
        {
            user = new User
            {
                Username = masterStudent.UniversityStudentId,
                PhoneNumber = masterStudent.MobileNumber,
                IsPhoneVerified = false,
                IsActive = false,
                MustChangePassword = true,
                PasswordHash = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
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

            // Keep the registered master-list number
            user.PhoneNumber = masterStudent.MobileNumber;

            await _userRepository.UpdateAsync(user);
        }

        // 7. Generate OTP
        var otp =
            await _otpService.GenerateAsync(user.UserId);

        // 8. Send OTP through Notify.lk
        await _smsService.SendAsync(
            masterStudent.MobileNumber,
            $"Your Campus Service Portal OTP is {otp}. It is valid for 5 minutes.");
    }

    public async Task VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp)
    {
        if (string.IsNullOrWhiteSpace(universityStudentId))
            throw new ArgumentException(
                "University Student ID is required.");

        if (string.IsNullOrWhiteSpace(mobileNumber))
            throw new ArgumentException(
                "Mobile number is required.");

        if (string.IsNullOrWhiteSpace(otp))
            throw new ArgumentException(
                "OTP is required.");

        // 1. Find student in Master Student List
        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    universityStudentId);

        if (masterStudent is null)
            throw new KeyNotFoundException(
                "Student was not found in the master student list.");

        // 2. Check mobile number
        if (masterStudent.MobileNumber != mobileNumber)
            throw new UnauthorizedAccessException(
                "Mobile number does not match the registered student record.");

        // 3. Find User
        var user =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        if (user is null)
            throw new KeyNotFoundException(
                "Student account was not found.");

        // 4. Verify OTP
        await _otpService.VerifyAsync(
            user.UserId,
            otp);

        // 5. Mark phone as verified
        user.IsPhoneVerified = true;
        user.IsActive = true;

        await _userRepository.UpdateAsync(user);

        // Student creation will be completed
        // after OTP verification flow is finalized.
    }
}