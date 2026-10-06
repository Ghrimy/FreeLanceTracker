using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ProjectDtos;

namespace FreeLanceTracker.Services.ProjectService;

/// <summary>
///     Service for managing projects.
/// </summary>
public interface IProjectService
{
    Task<ProjectDto> GetByIdAsync(int projectId, string userId);
    Task<IEnumerable<ProjectDto>> GetAllProjectsForClientAsync(int clientId, string userId);
    Task<ProjectDto> CreateAsync(ProjectDto projectdto, string userId);
    Task UpdateAsync(UpdateProjectDto projectdto, string userId);
    Task UpdateStatusAsync(UpdateProjectStatusDto projectdto, string userId);
    Task DeleteAsync(int projectId, string userId);
}