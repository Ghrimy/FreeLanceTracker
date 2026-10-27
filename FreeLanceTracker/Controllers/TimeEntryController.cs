using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.TimeEntryDtos;
using FreeLanceTracker.Services.TimeEntryService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/time-entry")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TimeEntryController(ITimeEntryService timeEntryService) : ControllerBase
{
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetAllForProjectAsync(int projectId)
    {
        var timeEntries = await timeEntryService.GetAllForProjectAsync(projectId, GetUserId());
        return Ok(timeEntries);
    }

    [HttpGet("unbilled")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetUnbilledByProjectIdsAsync(
        [FromBody] IEnumerable<int> projectIds)
    {
        var timeEntries = await timeEntryService.GetUnbilledByProjectIdsAsync(projectIds, GetUserId());
        return Ok(timeEntries);
    }

    [HttpPost("{projectId:int}/log-time")]
    public async Task<ActionResult<TimeEntryDto>> LogTimeAsync(int projectId, TimeEntry timeEntry)
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