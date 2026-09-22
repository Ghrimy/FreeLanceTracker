using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FreeLanceTracker.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [StringLength(100)] public string? FirstName { get; set; }
    [StringLength(100)] public string? LastName { get; set; }
    [StringLength(100)] public string? CompanyName { get; set; }
    [StringLength(200)] public string? Address { get; set; }
    public ICollection<Client> Clients { get; set; } = new List<Client>();
}