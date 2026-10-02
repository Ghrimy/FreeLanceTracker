using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.ProjectService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/project")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<Project>> GetProjectIdAsync(int projectId)
    {
        var project = await projectService.GetByIdAsync(projectId, GetUserId());
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllProjectsForClientAsync(int clientId)
    {
        var projectList = await projectService.GetAllProjectsForClientAsync(clientId, GetUserId());
        return Ok(projectList);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> CreateAsync(Project project)
    {
        var createProject = await projectService.CreateAsync(project, GetUserId());
        return Ok(createProject);
    }

    [HttpPatch("{projectId:int}")]
    public async Task<ActionResult> UpdateAsync(int projectId, Project project)
    {
        if (projectId != project.ProjectId) return BadRequest("Route/body ID mismatch.");
        await projectService.UpdateAsync(project, GetUserId());
        return Ok();
    }

    [HttpPatch("{projectId:int}/update-status")]
    public async Task<ActionResult> UpdateStatusAsync(int projectId, ProjectStatus status)
    {
        await projectService.UpdateStatusAsync(projectId, status, GetUserId());
        return Ok();
    }

    [HttpDelete("{projectId:int}")]
    public async Task<ActionResult> DeleteAsync(int projectId)
    {
        await projectService.DeleteAsync(projectId, GetUserId());
        return Ok();
    }
}