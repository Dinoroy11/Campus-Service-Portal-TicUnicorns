using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.SystemSettings.DTOs;

public class UpdateReservationHoldMinutesDto
{
    [Range(1, 60)]
    public int Minutes { get; set; }
}
