namespace FreeLanceTracker.DTOs.ClientDtos;

public class UpdateClientDto
{
    public int ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Description { get; set; }
}