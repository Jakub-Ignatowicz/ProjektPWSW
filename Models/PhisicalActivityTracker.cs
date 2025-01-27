using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class PhisicalActivityTracker : Tracker
    {

        [ForeignKey(nameof(ActivityTypeId))]
        public ActivityType ActivityType { get; set; } = null!;

        public int ActivityTypeId { get; set; }
    }
}
