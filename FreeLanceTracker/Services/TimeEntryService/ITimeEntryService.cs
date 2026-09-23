using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.TimeEntryService;

/// <summary>
/// Service for managing time entries.
/// </summary>
public interface ITimeEntryService
{
    Task<IEnumerable<TimeEntry>> GetAllForProjectAsync(int projectId, string userId);
    Task<IEnumerable<TimeEntry>> GetUnbilledByProjectIdsAsync(IEnumerable<int> projectIds, string userId);
    Task<TimeEntry> LogTimeAsync(TimeEntry entry, string userId);
    Task UpdateAsync(TimeEntry entry, string userId);
    Task DeleteAsync(int timeEntryId, string userId);
}