using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.DTOs.TimeEntryDtos;

namespace FreeLanceTracker.DTOs.ProjectDtos;

public class ProjectDto
{
    [Required] public int ProjectId { get; set; }
    [Required] [StringLength(50)] public string Name { get; set; } = string.Empty;
    [StringLength(200)] public string? Description { get; set; }
    [Required] public ProjectStatus Status { get; set; }
    [Required] public decimal HourlyRate { get; set; }
    [Required] public DateTime StartDate { get; set; }

    //relationships
    [Required] public int ClientId { get; set; }
    [Required] public ClientDto Client { get; set; }

    [Required] public ICollection<TimeEntryDto> TimeEntries { get; set; } = new List<TimeEntryDto>();
}