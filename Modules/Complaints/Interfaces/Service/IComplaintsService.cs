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

    // Complaints
    Task<List<ComplaintDto>> GetAllComplaintsAsync();

    Task<ComplaintDto?> GetComplaintByIdAsync(
        int complaintId);

    Task<List<ComplaintDto>> GetComplaintsByStudentIdAsync(
        int studentId);

    Task<ComplaintDto> CreateComplaintAsync(
        CreateComplaintDto dto);

    Task UpdateComplaintAsync(
        int complaintId,
        UpdateComplaintDto dto);

    // Status History
    Task<List<ComplaintStatusHistoryDto>>
        GetComplaintStatusHistoryAsync(int complaintId);
}