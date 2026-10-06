using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;

namespace FreeLanceTracker.Services.ClientService;

/// <summary>
///     Service for managing clients.
/// </summary>
public interface IClientService
{
    Task<ClientDto> GetClientIdAsync(int clientId, string userId);
    Task<IEnumerable<GetAllClientDto>> GetAllForUserAsync(string userId);
    Task<Client> CreateAsync(CreateClientDto client, string userId);
    Task UpdateAsync(UpdateClientDto client, string userId);
    Task ArchiveAsync(int clientId, string userId);
}