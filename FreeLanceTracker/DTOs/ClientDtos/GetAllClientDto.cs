using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.DTOs.ClientDtos;

public class GetAllClientDto
{
    [Required] public int ClientId { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Company { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
}