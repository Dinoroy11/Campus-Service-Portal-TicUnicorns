namespace CampusServicePortal.Modules.Gym.DTOs
{
    public class GymDto
    {
        public int GymId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}