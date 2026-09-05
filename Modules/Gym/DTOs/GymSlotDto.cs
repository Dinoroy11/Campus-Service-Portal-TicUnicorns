namespace CampusServicePortal.Modules.Gym.DTOs
{
    public class GymSlotDto
    {
        public int SlotId { get; set; }

        public int GymId { get; set; }

        public DateTime SlotDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int MaxCapacity { get; set; }

        public bool IsAvailable { get; set; }
    }
}