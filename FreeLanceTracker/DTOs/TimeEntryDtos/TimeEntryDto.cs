using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.DTOs.ProjectDtos;

namespace FreeLanceTracker.DTOs.TimeEntryDtos;

public class TimeEntryDto
{
    [Required] public int TimeEntryId { get; set; }
    [Required] public DateTime Date { get; set; }
    [Required] public decimal Hours { get; set; }
    [Required] [StringLength(200)] public string? Description { get; set; }
    [Required] public bool IsBillable { get; set; }
    [Required] public bool IsBilled { get; set; }

    //relationships
    [Required] public int ProjectId { get; set; }
    [Required] public ProjectDto Project { get; set; }
}