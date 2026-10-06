using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.DTOs.ClientDtos;

public class ClientDto
{
    [Required] public int ClientId { get; set; }
    [Required] [StringLength(200)] public string Name { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string Email { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string Company { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string Description { get; set; } = string.Empty;
}