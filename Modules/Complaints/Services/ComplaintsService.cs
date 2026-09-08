using CampusServicePortal.Modules.Complaints.DTOs;
using CampusServicePortal.Modules.Complaints.Entities;
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal.Modules.Complaints.Interfaces.Service;

namespace CampusServicePortal.Modules.Complaints.Services;

public class ComplaintsService : IComplaintsService
{
    private readonly IComplaintCategoryRepository _categoryRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IComplaintStatusHistoryRepository _historyRepository;

    public ComplaintsService(
        IComplaintCategoryRepository categoryRepository,
        IComplaintRepository complaintRepository,
        IComplaintStatusHistoryRepository historyRepository)
    {
        _categoryRepository = categoryRepository;
        _complaintRepository = complaintRepository;
        _historyRepository = historyRepository;
    }

    // =========================================================
    // Categories
    // =========================================================

    public async Task<List<ComplaintCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(x => new ComplaintCategoryDto
        {
            ComplaintCategoryId = x.ComplaintCategoryId,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<ComplaintCategoryDto?> GetCategoryByIdAsync(
        int complaintCategoryId)
    {
        var category =
            await _categoryRepository.GetByIdAsync(complaintCategoryId);

        if (category == null)
            return null;

        return new ComplaintCategoryDto
        {
            ComplaintCategoryId = category.ComplaintCategoryId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task<ComplaintCategoryDto> CreateCategoryAsync(
        CreateComplaintCategoryDto dto)
    {
        var category = new ComplaintCategory
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        await _categoryRepository.CreateAsync(category);

        return new ComplaintCategoryDto
        {
            ComplaintCategoryId = category.ComplaintCategoryId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task UpdateCategoryAsync(
        int complaintCategoryId,
        CreateComplaintCategoryDto dto)
    {
        var category =
            await _categoryRepository.GetByIdAsync(complaintCategoryId);

        if (category == null)
            throw new KeyNotFoundException("Complaint category not found.");

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _categoryRepository.UpdateAsync(category);
    }

    // =========================================================
    // Complaints
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

        if (complaint == null)
            return null;

        return MapComplaintToDto(complaint);
    }

    public async Task<List<ComplaintDto>> GetComplaintsByStudentIdAsync(
        int studentId)
    {
        var complaints =
            await _complaintRepository.GetByStudentIdAsync(studentId);

        return complaints.Select(MapComplaintToDto).ToList();
    }

    public async Task<ComplaintDto> CreateComplaintAsync(
        CreateComplaintDto dto)
    {
        var categoryExists =
            await _categoryRepository.ExistsAsync(dto.CategoryId);

        if (!categoryExists)
            throw new KeyNotFoundException(
                "Complaint category not found.");

        var complaint = new Complaint
        {
            CategoryId = dto.CategoryId,
            StudentId = dto.StudentId,
            Title = dto.Title,
            Description = dto.Description,
            Status = "Submitted",
            ActionRemarks = string.Empty,
            StatusChangedBy = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        await _complaintRepository.CreateAsync(complaint);

        return MapComplaintToDto(complaint);
    }

    public async Task UpdateComplaintAsync(
        int complaintId,
        UpdateComplaintDto dto)
    {
        var complaint =
            await _complaintRepository.GetByIdAsync(complaintId);

        if (complaint == null)
            throw new KeyNotFoundException("Complaint not found.");

        complaint.Status = dto.Status;
        complaint.ActionRemarks = dto.ActionRemarks;
        complaint.StatusChangedBy = dto.StatusChangedBy;
        complaint.UpdatedAt = DateTime.UtcNow;

        await _complaintRepository.UpdateAsync(complaint);

        var history = new ComplaintStatusHistory
        {
            ComplaintId = complaint.ComplaintId,
            Status = complaint.Status,
            Remarks = complaint.ActionRemarks,
            ChangedByUserId = dto.StatusChangedBy ?? 0,
            ChangedAt = DateTime.UtcNow
        };

        await _historyRepository.CreateAsync(history);
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

        var history =
            await _historyRepository.GetByComplaintIdAsync(complaintId);

        return history.Select(x => new ComplaintStatusHistoryDto
        {
            ComplaintStatusHistoryId =
                x.ComplaintStatusHistoryId,

            ComplaintId = x.ComplaintId,
            Status = x.Status,
            Remarks = x.Remarks,
            ChangedByUserId = x.ChangedByUserId,
            ChangedAt = x.ChangedAt
        }).ToList();
    }

    // =========================================================
    // Mapping
    // =========================================================

    private static ComplaintDto MapComplaintToDto(
        Complaint complaint)
    {
        return new ComplaintDto
        {
            ComplaintId = complaint.ComplaintId,
            CategoryId = complaint.CategoryId,
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