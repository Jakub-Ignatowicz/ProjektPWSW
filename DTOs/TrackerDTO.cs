namespace Projekt.DTOs;

public class TrackerDTO
{
    public float TargetValue { get; set; }
    public float Progress { get; set; } = 0;
    public string TrackerType { get; set; } = String.Empty;
    public int? ActivityTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
}
