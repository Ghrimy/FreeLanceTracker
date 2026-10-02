using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDTO;
using FreeLanceTracker.Middleware;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.ClientService;

public class ClientService(ApplicationDbContext context, IMapper clientMapper) : IClientService
{
    public async Task<GetClientDto> GetClientIdAsync(int clientId, string userId)
    {
        var client = await context.Clients
            .Where(c => c.ClientId == clientId)
            .Where(c => c.UserId == userId).FirstOrDefaultAsync();

        if (client is null) throw new NotFoundException("Client not found");
        var dto = clientMapper.Map<GetClientDto>(client);

        return dto;
    }

    public async Task<IEnumerable<GetAllClientDto>> GetAllForUserAsync(string userId)
    {
        var allClient = await context.Clients
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var dto = clientMapper.Map<IEnumerable<GetAllClientDto>>(allClient);
        return dto;
    }

    public async Task<Client> CreateAsync(CreateClientDto client, string userId)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) throw new NotFoundException("User not found");

        var newClient = new Client
        {
            Name = client.Name,
            Email = client.Email,
            Company = client.Company,
            Description = client.Description,
            UserId = userId
        };

        context.Clients.Add(newClient);
        await context.SaveChangesAsync();
        return newClient;
    }

    public async Task UpdateAsync(UpdateClientDto client, string userId)
    {
        var existingClient = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == client.ClientId && c.UserId == userId);

        if (existingClient is null) throw new NotFoundException("Client not found");

        existingClient.Name = client.Name;
        existingClient.Email = client.Email;
        existingClient.Company = client.Company;
        existingClient.Description = client.Description;
        await context.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int clientId, string userId)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == clientId && c.UserId == userId);

        if (client is null) throw new NotFoundException("Client not found");

        client.IsArchived = true;
        await context.SaveChangesAsync();
    }
}