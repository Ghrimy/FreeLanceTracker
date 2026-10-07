using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.Services.ClientService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/client")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ClientController(IClientService clientService) : ControllerBase
{
    [HttpGet("{clientId:int}")]
    public async Task<ActionResult<ClientDto>> GetClientIdAsync(int clientId)
    {
        var client = await clientService.GetClientIdAsync(clientId, GetUserId());
        return client is null ? NotFound() : Ok(client);
    }


    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<GetAllClientDto>>> GetAllForUserAsync()
    {
        var clientList = await clientService.GetAllForUserAsync(GetUserId());
        return Ok(clientList);
    }


    [HttpPost]
    public async Task<ActionResult<CreateClientDto>> CreateAsync(CreateClientDto clientDto)
    {
        var created = await clientService.CreateAsync(clientDto, GetUserId());
        return Ok(created);
    }


    [HttpPatch("{clientId:int}")]
    public async Task<ActionResult> UpdateAsync(int clientId, UpdateClientDto client)
    {
        if (client.ClientId != clientId) return BadRequest("Route/body ID mismatch.");

        await clientService.UpdateAsync(client, GetUserId());
        return Ok();
    }

    [HttpPost("{clientId:int}/archive")]
    public async Task<ActionResult> ArchiveAsync(int clientId)
    {
        await clientService.ArchiveAsync(clientId, GetUserId());
        return Ok();
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    }
}