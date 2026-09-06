using CampusServicePortal.Modules.Labs.Entities;

namespace CampusServicePortal.Modules.Labs.Interfaces.Repository;

public interface ILabTimeSlotRepository
{
    Task<List<LabTimeSlot>> GetByLabIdAsync(int labId);

    Task<LabTimeSlot?> GetByIdAsync(int timeSlotId);

    Task<LabTimeSlot> CreateAsync(LabTimeSlot timeSlot);

    Task UpdateAsync(LabTimeSlot timeSlot);

    Task<bool> ExistsAsync(int timeSlotId);
}