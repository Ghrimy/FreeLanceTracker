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

    public async Task<IEnumerable<ProjectDto>> GetRecentForUserAsync(string userId)
    {
        var projects = await context.Projects.AsNoTracking()
            .Include(p => p.Client)
            .Where(p => p.Client != null && p.Client.UserId == userId && !p.Client.IsArchived)
            .OrderByDescending(p => p.UpdatedAtUtc ?? p.CreatedAtUtc)
            .ThenByDescending(p => p.ProjectId)
            .Take(6)
            .ToListAsync();

        return mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto project, string userId)
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
            Status = project.Status,
            HourlyRate = project.HourlyRate,
            StartDate = project.StartDate,
            CreatedAtUtc = DateTime.UtcNow,
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
        existing.Status = project.Status;
        existing.HourlyRate = project.HourlyRate;
        existing.StartDate = project.StartDate;
        existing.UpdatedAtUtc = DateTime.UtcNow;
        context.Projects.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(UpdateProjectStatusDto projectdto, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectdto.ProjectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new NotFoundException("Project not found");
        project.Status = projectdto.Status;
        project.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, string userId)
    {
        var project = await context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new NotFoundException("Project not found");
        if (await context.TimeEntries.AnyAsync(t => t.ProjectId == projectId)
            || await context.InvoiceLineItems.AnyAsync(i => i.ProjectId == projectId))
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException("Remove unbilled time entries first. Keep projects with billing history and mark them completed instead.");
        }
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}