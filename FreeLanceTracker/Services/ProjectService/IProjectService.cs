using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.ProjectService;

/// <summary>
/// Service for managing projects.
/// </summary>
public interface IProjectService
{
    Task<Project?> GetByIdAsync(int projectId, string userId);
    Task<IEnumerable<Project>> GetAllProjectsForClientAsync(int clientId, string userId);
    Task<Project> CreateAsync(Project project, string userId);
    Task UpdateAsync(Project project, string userId);
    Task UpdateStatusAsync(int projectId, ProjectStatus status, string userId);
    Task DeleteAsync(int projectId, string userId);
}