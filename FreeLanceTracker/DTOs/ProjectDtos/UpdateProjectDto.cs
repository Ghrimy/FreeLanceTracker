using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.Data;

namespace FreeLanceTracker.DTOs.ProjectDtos;

public class UpdateProjectDto
{
    [Required] public int ProjectId { get; set; }
    [Required] [StringLength(50)] public string Name { get; set; } = string.Empty;
    [StringLength(200)] public string? Description { get; set; }
    [Required] public ProjectStatus Status { get; set; }
    [Required] public decimal HourlyRate { get; set; }
    [Required] public DateTime StartDate { get; set; }
}