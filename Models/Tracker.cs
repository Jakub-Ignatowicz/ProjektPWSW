using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public abstract class Tracker
    {
        [Key]
        public int TrackerID { get; set; }
        public float TargetValue { get; set; }
        public float Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public bool checkGoalCompletion() { return Progress >= TargetValue; }
        public void markProgress(float amount) { Progress += amount; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; } = null!;
        public int UserID { get; set; }
    }
}
