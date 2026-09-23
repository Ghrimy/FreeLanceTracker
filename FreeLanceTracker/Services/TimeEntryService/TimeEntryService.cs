using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.TimeEntryService;

public class TimeEntryService : ITimeEntryService
{
    public async Task<IEnumerable<TimeEntry>> GetAllForProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TimeEntry>> GetUnbilledByProjectIdsAsync(IEnumerable<int> projectIds)
    {
        throw new NotImplementedException();
    }

    public async Task<TimeEntry> LogTimeAsync(TimeEntry entry)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(TimeEntry entry)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int timeEntryId)
    {
        throw new NotImplementedException();
    }
}