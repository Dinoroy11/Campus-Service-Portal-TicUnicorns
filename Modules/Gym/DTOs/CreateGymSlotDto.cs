namespace CampusServicePortal.Modules.Gym.DTOs
{
    public class CreateGymSlotDto
    {
        public int GymId { get; set; }

        public DateTime SlotDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int MaxCapacity { get; set; }
    }
}