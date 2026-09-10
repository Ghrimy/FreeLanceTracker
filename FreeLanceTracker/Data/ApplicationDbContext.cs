using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Configure table relationships here
        //TODO: Configure table properties here (e.g. decimal places etc.)
        base.OnModelCreating(modelBuilder); // Important for Identity
        
        
        modelBuilder.Entity<TimeEntry>().Property(d => d.Date).HasColumnType("date");
        modelBuilder.Entity<TimeEntry>().Property(d => d.Hours).HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<Project>().Property(d => d.StartDate).HasColumnType("date");
        modelBuilder.Entity<Project>().Property(h => h.HourlyRate).HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<Invoice>().Property(d => d.DueDate).HasColumnType("date");
        modelBuilder.Entity<Invoice>().Property(d => d.IssueDate).HasColumnType("date");
        
        modelBuilder.Entity<InvoiceLineItem>().Property(d => d.Quantity).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<InvoiceLineItem>().Property(d => d.UnitPrice).HasColumnType("decimal(18,2)");
        

    }
    
}