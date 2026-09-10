using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.Data;

public class TimeEntry
{
    //properties
    [Key] public int TimeEntryId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    [StringLength(200)] public string? Description { get; set; } = string.Empty;
    public bool IsBillable { get; set; }
    
    //relationships
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}