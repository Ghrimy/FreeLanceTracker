using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.Data;

namespace FreeLanceTracker.DTOs.ProjectDtos;

public class UpdateProjectStatusDto
{
    [Required] public int ProjectId { get; set; }
    [Required] public ProjectStatus Status { get; set; }
}