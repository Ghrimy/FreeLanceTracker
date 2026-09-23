using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.TimeEntryService;

/// <summary>
/// Service for managing time entries.
/// </summary>
public interface ITimeEntryService
{
    Task<IEnumerable<TimeEntry>> GetAllForProjectAsync(int projectId);
    Task<IEnumerable<TimeEntry>> GetUnbilledByProjectIdsAsync(IEnumerable<int> projectIds);
    Task<TimeEntry> LogTimeAsync(TimeEntry entry);
    Task UpdateAsync(TimeEntry entry);
    Task DeleteAsync(int timeEntryId);
}