using CampusServicePortal.Modules.Labs.Entities;

namespace CampusServicePortal.Modules.Labs.Interfaces.Repository;

public interface ILabSeatRepository
{
    Task<List<LabSeat>> GetByLabIdAsync(int labId);

    Task<LabSeat?> GetByIdAsync(int labSeatId);

    Task<LabSeat> CreateAsync(LabSeat labSeat);

    Task UpdateAsync(LabSeat labSeat);

    Task<bool> ExistsAsync(int labSeatId);
}