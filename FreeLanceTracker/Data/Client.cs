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

    //One client can have many projects
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    //One client can have many invoices 
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}