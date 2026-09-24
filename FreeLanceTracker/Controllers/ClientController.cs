using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.ClientService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/client")]
[Authorize]
public class ClientController(IClientService clientService) : ControllerBase
{

    [HttpGet("{clientId:int}")]
    public async Task<ActionResult<Client>> GetClientIdAsync(int clientId)
    {
        var client = await clientService.GetClientIdAsync(clientId, GetUserId());
        return client is null ? NotFound() : Ok(client);
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllForUserAsync()
    {
        var clientList = await clientService.GetAllForUserAsync(GetUserId());
        return Ok(clientList);
    }
    
    [HttpPost]
    public async Task<ActionResult<Client>> CreateAsync(Client client)
    {

        var createClient = await clientService.CreateAsync(client, GetUserId());
        return Ok(createClient);
    }

    [HttpPatch("{clientId:int}")]
    public async Task<ActionResult> UpdateAsync(Client client)
    {
        if (client.ClientId == 0) throw new Exception("Client ID is required");
        
        await clientService.UpdateAsync(client, GetUserId());
        return Ok();
    }

    [HttpPost("{clientId:int}/archive")]
    public async Task<ActionResult> ArchiveAsync(int clientId)
    {
        await clientService.ArchiveAsync(clientId, GetUserId());
        return Ok();
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();


}