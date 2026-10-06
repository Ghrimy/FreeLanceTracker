using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ProjectDtos;
using FreeLanceTracker.Middleware;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.ProjectService;

public class ProjectService(ApplicationDbContext context, IMapper mapper) : IProjectService
{
    public async Task<ProjectDto> GetByIdAsync(int projectId, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
        
        var dto = mapper.Map<ProjectDto>(project);
        return dto;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsForClientAsync(int clientId, string userId)
    {
        var project = await context.Projects
            .Where(u => u.ClientId == clientId && u.Client != null && u.Client.UserId == userId)
            .ToListAsync();

        if (project is null) throw new NotFoundException("Project not found");
        var dtos = mapper.Map<IEnumerable<ProjectDto>>(project);
        return dtos;
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto project, string userId)
    {
        var client =
            await context.Clients.FirstOrDefaultAsync(c => c.ClientId == project.ClientId && c.UserId == userId);
        if (client is null) throw new NotFoundException("Client not found");

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
        
        var dto = mapper.Map<ProjectDto>(newProject);
        return dto;
    }

    public async Task UpdateAsync(UpdateProjectDto project, string userId)
    {
        var existing = await context.Projects
            .FirstOrDefaultAsync(p =>
                p.ProjectId == project.ProjectId && p.Client != null && p.Client.UserId == userId);
        if (existing is null) throw new NotFoundException("Project not found");

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.HourlyRate = project.HourlyRate;
        existing.StartDate = project.StartDate;
        context.Projects.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(UpdateProjectStatusDto projectdto, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectdto.ProjectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new NotFoundException("Project not found");
        project.Status = projectdto.Status;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new NotFoundException("Project not found");
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}