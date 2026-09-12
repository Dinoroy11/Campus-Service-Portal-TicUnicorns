using CampusServicePortal.Modules.Complaints.DTOs;
using CampusServicePortal.Modules.Complaints.Entities;
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal.Modules.Complaints.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal.Modules.Complaints.Services;

public class ComplaintsService : IComplaintsService
{
    private readonly IComplaintCategoryRepository _categoryRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IComplaintStatusHistoryRepository _historyRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public ComplaintsService(
        IComplaintCategoryRepository categoryRepository,
        IComplaintRepository complaintRepository,
        IComplaintStatusHistoryRepository historyRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _categoryRepository = categoryRepository;
        _complaintRepository = complaintRepository;
        _historyRepository = historyRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    // =========================================================
    // Categories
    // =========================================================

    public async Task<List<ComplaintCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories
            .OrderBy(x => x.Name)
            .Select(MapCategoryToDto)
            .ToList();
    }

    public async Task<ComplaintCategoryDto?> GetCategoryByIdAsync(
        int complaintCategoryId)
    {
        var category =
            await _categoryRepository.GetByIdAsync(complaintCategoryId);

        return category == null
            ? null
            : MapCategoryToDto(category);
    }

    public async Task<ComplaintCategoryDto> CreateCategoryAsync(
        CreateComplaintCategoryDto dto)
    {
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Complaint category name is required.");

        var categories = await _categoryRepository.GetAllAsync();
        if (categories.Any(x =>
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "A complaint category with this name already exists.");
        }

        var category = new ComplaintCategory
        {
            Name = name,
            Description = dto.Description.Trim(),
            IsActive = true
        };

        await _categoryRepository.CreateAsync(category);

        return MapCategoryToDto(category);
    }

    public async Task UpdateCategoryAsync(
        int complaintCategoryId,
        CreateComplaintCategoryDto dto)
    {
        var category =
            await _categoryRepository.GetByIdAsync(complaintCategoryId);

        if (category == null)
            throw new KeyNotFoundException("Complaint category not found.");

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Complaint category name is required.");

        var categories = await _categoryRepository.GetAllAsync();
        if (categories.Any(x =>
            x.ComplaintCategoryId != complaintCategoryId &&
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "A complaint category with this name already exists.");
        }

        category.Name = name;
        category.Description = dto.Description.Trim();

        await _categoryRepository.UpdateAsync(category);
    }

    // =========================================================
    // ADMIN: Complaints
    // =========================================================

    public async Task<List<ComplaintDto>> GetAllComplaintsAsync()
    {
        var complaints = await _complaintRepository.GetAllAsync();
        return complaints.Select(MapComplaintToDto).ToList();
    }

    public async Task<ComplaintDto?> GetComplaintByIdAsync(
        int complaintId)
    {
        var complaint =
            await _complaintRepository.GetByIdAsync(complaintId);

        return complaint == null
            ? null
            : MapComplaintToDto(complaint);
    }

    public async Task<ComplaintDto> UpdateComplaintStatusAsync(
        int complaintId,
        int changedByUserId,
        UpdateComplaintDto dto)
    {
        var complaint =
            await _complaintRepository.GetByIdAsync(complaintId)
            ?? throw new KeyNotFoundException("Complaint not found.");

        var newStatus = NormalizeStatus(dto.Status);

        if (newStatus == "Submitted")
            throw new InvalidOperationException(
                "Admin cannot move a complaint back to Submitted.");

        if (newStatus == "Closed")
            throw new InvalidOperationException(
                "Only the student can close a resolved complaint.");

        ValidateAdminTransition(complaint.Status, newStatus);

        complaint.Status = newStatus;
        complaint.ActionRemarks = dto.ActionRemarks?.Trim() ?? string.Empty;
        complaint.StatusChangedBy = changedByUserId;
        complaint.UpdatedAt = DateTime.UtcNow;

        await _complaintRepository.UpdateAsync(complaint);

        await AddHistoryAsync(
            complaint.ComplaintId,
            complaint.Status,
            complaint.ActionRemarks,
            changedByUserId);

        await NotifyStudentAsync(complaint);

        return MapComplaintToDto(complaint);
    }

    // =========================================================
    // STUDENT: Complaints
    // =========================================================

    public async Task<List<ComplaintDto>> GetMyComplaintsAsync(
        int currentUserId)
    {
        var student = await GetStudentByUserIdAsync(currentUserId);

        var complaints = await _complaintRepository
            .GetByStudentIdAsync(student.StudentId);

        return complaints.Select(MapComplaintToDto).ToList();
    }

    public async Task<ComplaintDto?> GetMyComplaintByIdAsync(
        int currentUserId,
        int complaintId)
    {
        var student = await GetStudentByUserIdAsync(currentUserId);
        var complaint = await _complaintRepository.GetByIdAsync(complaintId);

        if (complaint == null || complaint.StudentId != student.StudentId)
            return null;

        return MapComplaintToDto(complaint);
    }

    public async Task<ComplaintDto> CreateComplaintAsync(
        int currentUserId,
        CreateComplaintDto dto)
    {
        var student = await GetStudentByUserIdAsync(currentUserId);

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException("Complaint category not found.");

        if (!category.IsActive)
            throw new InvalidOperationException(
                "This complaint category is inactive.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Complaint title is required.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Complaint description is required.");

        var complaint = new Complaint
        {
            CategoryId = dto.CategoryId,
            StudentId = student.StudentId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Status = "Submitted",
            ActionRemarks = string.Empty,
            StatusChangedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            Category = category
        };

        await _complaintRepository.CreateAsync(complaint);

        await AddHistoryAsync(
            complaint.ComplaintId,
            "Submitted",
            "Complaint submitted by student.",
            currentUserId);

        return MapComplaintToDto(complaint);
    }

    public async Task<ComplaintDto> ConfirmResolutionAsync(
        int currentUserId,
        int complaintId,
        ConfirmComplaintResolutionDto dto)
    {
        var student = await GetStudentByUserIdAsync(currentUserId);

        var complaint = await _complaintRepository.GetByIdAsync(complaintId)
            ?? throw new KeyNotFoundException("Complaint not found.");

        if (complaint.StudentId != student.StudentId)
            throw new UnauthorizedAccessException(
                "You can only close your own complaint.");

        if (!complaint.Status.Equals(
            "Resolved",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only a resolved complaint can be confirmed and closed.");
        }

        complaint.Status = "Closed";
        complaint.ActionRemarks = string.IsNullOrWhiteSpace(dto.Remarks)
            ? "Resolution confirmed by student."
            : dto.Remarks.Trim();
        complaint.StatusChangedBy = currentUserId;
        complaint.UpdatedAt = DateTime.UtcNow;

        await _complaintRepository.UpdateAsync(complaint);

        await AddHistoryAsync(
            complaint.ComplaintId,
            "Closed",
            complaint.ActionRemarks,
            currentUserId);

        return MapComplaintToDto(complaint);
    }

    // =========================================================
    // Status History
    // =========================================================

    public async Task<List<ComplaintStatusHistoryDto>>
        GetComplaintStatusHistoryAsync(int complaintId)
    {
        var complaintExists =
            await _complaintRepository.ExistsAsync(complaintId);

        if (!complaintExists)
            throw new KeyNotFoundException("Complaint not found.");

        return await GetHistoryAsync(complaintId);
    }

    public async Task<List<ComplaintStatusHistoryDto>>
        GetMyComplaintStatusHistoryAsync(
            int currentUserId,
            int complaintId)
    {
        var student = await GetStudentByUserIdAsync(currentUserId);
        var complaint = await _complaintRepository.GetByIdAsync(complaintId)
            ?? throw new KeyNotFoundException("Complaint not found.");

        if (complaint.StudentId != student.StudentId)
            throw new UnauthorizedAccessException(
                "You can only view history for your own complaint.");

        return await GetHistoryAsync(complaintId);
    }

    // =========================================================
    // Helpers
    // =========================================================

    private async Task<CampusServicePortal_TicUnicorns.Modules.Students.Entities.Student>
        GetStudentByUserIdAsync(int currentUserId)
    {
        return await _studentRepository.GetByUserIdAsync(currentUserId)
            ?? throw new InvalidOperationException(
                "Student profile was not found for the logged-in user.");
    }

    private async Task AddHistoryAsync(
        int complaintId,
        string status,
        string remarks,
        int changedByUserId)
    {
        await _historyRepository.CreateAsync(new ComplaintStatusHistory
        {
            ComplaintId = complaintId,
            Status = status,
            Remarks = remarks,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow
        });
    }

    private async Task<List<ComplaintStatusHistoryDto>>
        GetHistoryAsync(int complaintId)
    {
        var history =
            await _historyRepository.GetByComplaintIdAsync(complaintId);

        return history.Select(x => new ComplaintStatusHistoryDto
        {
            ComplaintStatusHistoryId = x.ComplaintStatusHistoryId,
            ComplaintId = x.ComplaintId,
            Status = x.Status,
            Remarks = x.Remarks,
            ChangedByUserId = x.ChangedByUserId,
            ChangedAt = x.ChangedAt
        }).ToList();
    }

    private async Task NotifyStudentAsync(Complaint complaint)
    {
        var student = await _studentRepository.GetByIdAsync(complaint.StudentId);

        if (student?.UserId is not int userId)
            return;

        var message = complaint.Status switch
        {
            "UnderReview" => "Your complaint is now under review.",
            "InProgress" => "Your complaint is now in progress.",
            "Resolved" => "Your complaint has been resolved. Please review it and confirm the resolution.",
            "Rejected" => "Your complaint has been rejected. Please review the administrator remarks.",
            _ => $"Your complaint status changed to {complaint.Status}."
        };

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId,
            Title = "Complaint Status Updated",
            Message = message,
            ReferenceType = "Complaint",
            ReferenceId = complaint.ComplaintId
        });
    }

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Complaint status is required.");

        return status.Trim().ToLowerInvariant() switch
        {
            "submitted" => "Submitted",
            "underreview" => "UnderReview",
            "under review" => "UnderReview",
            "inprogress" => "InProgress",
            "in progress" => "InProgress",
            "resolved" => "Resolved",
            "rejected" => "Rejected",
            "closed" => "Closed",
            _ => throw new ArgumentException("Invalid complaint status.")
        };
    }

    private static void ValidateAdminTransition(
        string currentStatus,
        string newStatus)
    {
        var current = NormalizeStatus(currentStatus);

        if (current == newStatus)
            throw new InvalidOperationException(
                $"Complaint is already {newStatus}.");

        var allowed = current switch
        {
            "Submitted" => new[] { "UnderReview", "InProgress", "Rejected" },
            "UnderReview" => new[] { "InProgress", "Rejected" },
            "InProgress" => new[] { "Resolved", "Rejected" },
            "Resolved" => Array.Empty<string>(),
            "Rejected" => Array.Empty<string>(),
            "Closed" => Array.Empty<string>(),
            _ => Array.Empty<string>()
        };

        if (!allowed.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid complaint status transition: {current} -> {newStatus}.");
        }
    }

    private static ComplaintCategoryDto MapCategoryToDto(
        ComplaintCategory category)
    {
        return new ComplaintCategoryDto
        {
            ComplaintCategoryId = category.ComplaintCategoryId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    private static ComplaintDto MapComplaintToDto(Complaint complaint)
    {
        return new ComplaintDto
        {
            ComplaintId = complaint.ComplaintId,
            CategoryId = complaint.CategoryId,
            CategoryName = complaint.Category?.Name ?? string.Empty,
            StudentId = complaint.StudentId,
            Title = complaint.Title,
            Description = complaint.Description,
            Status = complaint.Status,
            ActionRemarks = complaint.ActionRemarks,
            StatusChangedBy = complaint.StatusChangedBy,
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt
        };
    }
}
