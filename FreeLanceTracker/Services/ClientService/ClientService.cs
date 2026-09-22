using FreeLanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.ClientService;

public class ClientService(ApplicationDbContext context) : IClientService
{
    public async Task<Client?> GetClientIdAsync(int clientId, string userId)
    {
        var client =  context.Clients
            .Where(c => c.ClientId == clientId)
            .Where(c => c.UserId == userId).FirstOrDefaultAsync();

        return await client;
    }

    public async Task<IEnumerable<Client>> GetAllForUserAsync(string userId)
    {
        return await context.Clients
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Client> CreateAsync(Client client, string userId)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) throw new Exception("User not found");

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

    public async Task UpdateAsync(Client client, string userId)
    {
        var existingClient = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == client.ClientId && c.UserId == userId);

        if (existingClient is null) throw new Exception("Client not found");

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

        if (client is null) throw new Exception("Client not found");

        client.IsArchived = true;
        await context.SaveChangesAsync();
    }
}