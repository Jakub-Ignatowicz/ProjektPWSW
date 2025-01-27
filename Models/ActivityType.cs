using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class ActivityType
    {
        [Key]
        public int ActivityID { get; set; }
        public string Name { get; set; } = String.Empty;
        public Type Type { get; set; }

        public ICollection<PhisicalActivityTracker> PhysicalActivities { get; set; } = new HashSet<PhisicalActivityTracker>();
    }
    public enum Type
    {
        Duration,
        Repetition,
    }
}
