using System.ComponentModel.DataAnnotations;

namespace CampusServicePortal.Modules.SystemSettings.Entities
{
    public class SystemSetting
    {
        [Key]
        public int SettingId { get; set; }

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}