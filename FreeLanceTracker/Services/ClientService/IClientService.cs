using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.ClientService;

/// <summary>
/// Service for managing clients.
/// </summary>
public interface IClientService
{
    Task<Client?> GetClientIdAsync(int clientId, string userId);
    Task<IEnumerable<Client>> GetAllForClientAsync(string userId);
    Task<Client> CreateAsync(Client client, string userId);
    Task<Client> UpdateAsync(Client client, string userId);
    Task ArchiveAsync(int clientId);
}