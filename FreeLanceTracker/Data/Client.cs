using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.Data;

public class Client
{
    //properties
    [Key] public int ClientId { get; set; }

    [Required, StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)] public string? Company { get; set; }
    [StringLength(200)] public string? Description { get; set; }

    //relationships
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}