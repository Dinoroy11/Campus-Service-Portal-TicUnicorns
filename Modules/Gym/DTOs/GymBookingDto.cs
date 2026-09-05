using CampusServicePortal.Modules.Gym.Enums;

namespace CampusServicePortal.Modules.Gym.DTOs
{
    public class GymBookingDto
    {
        public int BookingId { get; set; }

        public int SlotId { get; set; }

        public int UserId { get; set; }

        public DateTime BookingDate { get; set; }

        public GymBookingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}