using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.ProjectService;

/// <summary>
/// Service for managing projects.
/// </summary>
public interface IProject
{
    Task<Project> GetProjectByIdAsync(int projectId, string userId);
    Task<IEnumerable<Project>> GetAllProjectsForClientAsync(int clientId);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task UpdateStatusAsync(int projectId, ProjectStatus status);
    Task DeleteAsync(int projectId);
}