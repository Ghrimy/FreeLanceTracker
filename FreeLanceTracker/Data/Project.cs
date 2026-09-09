using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeLanceTracker.Data;

public enum ProjectStatus { Active, Completed, OnHold }

public class Project
{
    //properties
    [Key] public int ProjectId { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)] public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime StartDate { get; set; }

    //relationships
    public int ClientId { get; set; }
    public Client? Client { get; set; }
}