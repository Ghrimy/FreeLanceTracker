using FreeLanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.ProjectService;

public class ProjectService(ApplicationDbContext context) : IProjectService
{
    public async Task<Project?> GetByIdAsync(int projectId, string userId)
    {
        return await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
    }

    public async Task<IEnumerable<Project>> GetAllProjectsForClientAsync(int clientId, string userId)
    {
        var project = context.Projects
            .Where(u => u.ClientId == clientId && u.Client != null && u.Client.UserId == userId)
            .ToListAsync();
        
        if(project is null) throw new Exception("Project not found");
        return await project;
    }

    public async Task<Project> CreateAsync(Project project, string userId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.ClientId == project.ClientId && c.UserId == userId);
        if (client is null) throw new Exception("Client not found");

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            Client = client,
            ClientId = client.ClientId,
            Status = ProjectStatus.Active,
            HourlyRate = project.HourlyRate,
            StartDate = project.StartDate,
            TimeEntries = new List<TimeEntry>()
        };
        
        context.Projects.Add(newProject);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project, string userId)
    {
        var existing = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId && p.Client != null && p.Client.UserId == userId);
        if (existing is null) throw new Exception("Project not found");
        
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.HourlyRate = project.HourlyRate;
        existing.StartDate = project.StartDate;
        context.Projects.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int projectId, ProjectStatus status, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new Exception("Project not found");
        project.Status = status;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new Exception("Project not found");
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}