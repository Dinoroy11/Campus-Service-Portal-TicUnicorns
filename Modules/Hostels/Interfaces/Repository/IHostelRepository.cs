using CampusServicePortal.Modules.Hostels.Entities;

namespace CampusServicePortal.Modules.Hostels.Repositories;

public interface IHostelRepository
{
    Task<List<Hostel>> GetHostelsAsync();

    Task<Hostel?> GetHostelBlueprintAsync(int hostelId);
}