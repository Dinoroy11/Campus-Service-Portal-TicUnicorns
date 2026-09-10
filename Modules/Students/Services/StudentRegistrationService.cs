using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using Microsoft.AspNetCore.Identity;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Services;

public class StudentRegistrationService : IStudentRegistrationService
{
    private readonly IStudentMasterListRepository _masterListRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IOtpService _otpService;

    private readonly PasswordHasher<User> _passwordHasher;

    public StudentRegistrationService(
        IStudentMasterListRepository masterListRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IStudentRepository studentRepository,
        IOtpService otpService)
    {
        _masterListRepository = masterListRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _studentRepository = studentRepository;
        _otpService = otpService;

        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task SendOtpAsync(StudentRegistrationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UniversityStudentId))
        {
            throw new ArgumentException(
                "University student ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.MobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.");
        }

        var masterStudent =
            await _masterListRepository
                .GetByUniversityStudentIdAsync(
                    dto.UniversityStudentId);

        if (masterStudent is null)
        {
            throw new KeyNotFoundException(
                "Student was not found in the master list.");
        }

        if (!masterStudent.IsActive)
        {
            throw new InvalidOperationException(
                "Student is not active in the master list.");
        }

        if (!string.Equals(
                masterStudent.MobileNumber,
                dto.MobileNumber,
                StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException(
                "The mobile number does not match the student record.");
        }

        var existingStudent =
            await _studentRepository
                .ExistsByMasterStudentIdAsync(
                    masterStudent.MasterStudentId);

        if (existingStudent)
        {
            throw new InvalidOperationException(
                "Student registration has already been completed.");
        }

        var existingUser =
            await _userRepository
                .GetByUsernameAsync(
                    masterStudent.UniversityStudentId);

        User user;

        if (existingUser is null)
        {
            user = new User
            {
                Username = masterStudent.UniversityStudentId,
                PhoneNumber = masterStudent.MobileNumber,
                IsPhoneVerified = false,
                IsActive = false,
                MustChangePassword = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                Guid.NewGuid().ToString());

            await _userRepository.AddAsync(user);
        }
        else
        {
            if (existingUser.IsPhoneVerified)
            {
                throw new InvalidOperationException(
                    "This user account is already verified.");
            }

            user = existingUser;

            user.PhoneNumber = masterStudent.MobileNumber;

            await _userRepository.UpdateAsync(user);
        }

        await _otpService.GenerateAsync(user.UserId);
    }

    public async Task VerifyOtpAsync(
        string universityStudentId,
        string mobileNumber,
        string otp)
    {
        throw new NotImplementedException(
            "OTP verification will be implemented in the next step.");
    }
}