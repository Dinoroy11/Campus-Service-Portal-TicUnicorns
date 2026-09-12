using CampusServicePortal.Modules.Complaints.DTOs;

namespace CampusServicePortal.Modules.Complaints.Interfaces.Service;

public interface IComplaintsService
{
    // Categories
    Task<List<ComplaintCategoryDto>> GetAllCategoriesAsync();

    Task<ComplaintCategoryDto?> GetCategoryByIdAsync(
        int complaintCategoryId);

    Task<ComplaintCategoryDto> CreateCategoryAsync(
        CreateComplaintCategoryDto dto);

    Task UpdateCategoryAsync(
        int complaintCategoryId,
        CreateComplaintCategoryDto dto);

    // Admin complaint access
    Task<List<ComplaintDto>> GetAllComplaintsAsync();

    Task<ComplaintDto?> GetComplaintByIdAsync(
        int complaintId);

    Task<ComplaintDto> UpdateComplaintStatusAsync(
        int complaintId,
        int changedByUserId,
        UpdateComplaintDto dto);

    // Student complaint access
    Task<List<ComplaintDto>> GetMyComplaintsAsync(
        int currentUserId);

    Task<ComplaintDto?> GetMyComplaintByIdAsync(
        int currentUserId,
        int complaintId);

    Task<ComplaintDto> CreateComplaintAsync(
        int currentUserId,
        CreateComplaintDto dto);

    Task<ComplaintDto> ConfirmResolutionAsync(
        int currentUserId,
        int complaintId,
        ConfirmComplaintResolutionDto dto);

    // Status history
    Task<List<ComplaintStatusHistoryDto>>
        GetComplaintStatusHistoryAsync(int complaintId);

    Task<List<ComplaintStatusHistoryDto>>
        GetMyComplaintStatusHistoryAsync(
            int currentUserId,
            int complaintId);
}
