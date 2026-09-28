using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.TimeEntryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/time-entry")]
[Authorize]
public class TimeEntryController(ITimeEntryService timeEntryService) : ControllerBase
{
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    
    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TimeEntry>>> GetAllForProjectAsync(int projectId)
    {
        var timeEntries = await timeEntryService.GetAllForProjectAsync(projectId, GetUserId());
        return Ok(timeEntries);
    }

    [HttpGet("unbilled")]
    public async Task<ActionResult<IEnumerable<TimeEntry>>> GetUnbilledByProjectIdsAsync(IEnumerable<int> projectIds)
    {
        var timeEntries = await timeEntryService.GetUnbilledByProjectIdsAsync(projectIds, GetUserId());
        return Ok(timeEntries);   
    }

    [HttpPost("{projectId:int}/log-time")]
    public async Task<ActionResult<TimeEntry>> LogTimeAsync(int projectId, TimeEntry timeEntry)
    {
        if (projectId != timeEntry.ProjectId) return BadRequest("Route/body project ID mismatch.");
        var createdTimeEntry = await timeEntryService.LogTimeAsync(timeEntry, GetUserId());
        return Ok(createdTimeEntry);
    }
    
    [HttpPatch("{timeEntryId:int}")]
    public async Task<ActionResult> UpdateAsync(int timeEntryId, TimeEntry timeEntry)
    {
        if (timeEntryId != timeEntry.TimeEntryId) return BadRequest("Route/body ID mismatch.");
        await timeEntryService.UpdateAsync(timeEntry, GetUserId());
        return Ok();
    }
    
    [HttpDelete("{timeEntryId:int}")]
    public async Task<ActionResult> DeleteAsync(int timeEntryId)
    {
        await timeEntryService.DeleteAsync(timeEntryId, GetUserId());
        return Ok();
    }
        
}