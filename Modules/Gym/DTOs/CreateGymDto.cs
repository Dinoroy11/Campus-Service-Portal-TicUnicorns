namespace CampusServicePortal.Modules.Gym.DTOs
{
    public class CreateGymDto
    {
        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}