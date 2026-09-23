using System.Net.Mime;
using FreeLanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.TimeEntryService;

public class TimeEntryService(ApplicationDbContext context) : ITimeEntryService
{
    public async Task<IEnumerable<TimeEntry>> GetAllForProjectAsync(int projectId, string userId)
    {
        var existing = context.TimeEntries.Where(t => t.ProjectId == projectId 
                                                      && t.Project != null
                                                      && t.Project.Client != null
                                                      && t.Project.Client.UserId == userId).ToListAsync();
        return await existing;
    }

    public async Task<IEnumerable<TimeEntry>> GetUnbilledByProjectIdsAsync(IEnumerable<int> projectIds, string userId)
    {
        //Load all time entries for userId
        var existing = context.TimeEntries
            .Where(t => t.Project != null 
                        && t.Project.Client != null 
                        && t.Project.Client.UserId == userId
                        && projectIds.Contains(t.ProjectId)
                        && !t.IsBilled).ToListAsync();
        
        return await existing;
        
    }

    public async Task<TimeEntry> LogTimeAsync(TimeEntry entry, string userId)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p => p.ProjectId == entry.ProjectId && p.Client != null && p.Client.UserId == userId);
        if (project is null) throw new Exception("Project not found");

        var newEntry = new TimeEntry
        {
            Date = entry.Date,
            Description = entry.Description,
            Hours = entry.Hours,
            IsBillable = entry.IsBillable,
            Project = project,
            ProjectId = project.ProjectId,
            IsBilled = false
        };
        
        context.TimeEntries.Add(newEntry);
        await context.SaveChangesAsync();
        return newEntry;
    }

    public async Task UpdateAsync(TimeEntry entry, string userId)
    {
        var existing = await context.TimeEntries
            .FirstOrDefaultAsync(t => t.TimeEntryId == entry.TimeEntryId
                                      && t.Project != null && t.Project.Client != null
                                      && t.Project.Client.UserId == userId);
        if (existing is null) throw new Exception("Time entry not found");

        if (existing.IsBilled)
            throw new InvalidOperationException("Cannot edit a time entry that has already been billed.");

        existing.Date = entry.Date;
        existing.Description = entry.Description;
        existing.Hours = entry.Hours;
        existing.IsBillable = entry.IsBillable;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int timeEntryId, string userId)
    {
        var existing = await context.TimeEntries
            .FirstOrDefaultAsync(t => t.TimeEntryId == timeEntryId 
                                      && t.Project != null && t.Project.Client != null 
                                      && t.Project.Client.UserId == userId);
        if (existing is null) throw new Exception("Time entry not found");
        
        if (existing.IsBilled)
            throw new InvalidOperationException("Cannot edit a time entry that has already been billed.");

        context.TimeEntries.Remove(existing);
        await context.SaveChangesAsync();
    }
}