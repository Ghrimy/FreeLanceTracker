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
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Configure table relationships here
        //TODO: Configure table properties here (e.g. decimal places etc.)
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TimeEntry>().Property(d => d.Date).HasColumnType("date");
        modelBuilder.Entity<TimeEntry>().Property(d => d.Hours).HasColumnType("decimal(6,2)");
        
        modelBuilder.Entity<Project>().Property(d => d.StartDate).HasColumnType("date");
        modelBuilder.Entity<Project>().Property(h => h.HourlyRate).HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<Invoice>().Property(d => d.DueDate).HasColumnType("date");
        modelBuilder.Entity<Invoice>().Property(d => d.IssueDate).HasColumnType("date");
        
        modelBuilder.Entity<InvoiceLineItem>().Property(q => q.Quantity).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<InvoiceLineItem>().Property(q => q.UnitPrice).HasColumnType("decimal(18,2)");
    
        //relationships
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Projects)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Project>()
            .HasMany(p => p.TimeEntries)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.LineItems)
            .WithOne(l => l.Invoice)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InvoiceLineItem>()
            .HasOne(li => li.Project)
            .WithMany()
            .HasForeignKey(li => li.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<InvoiceLineItem>()
            .HasOne(li => li.TimeEntry)
            .WithMany()
            .HasForeignKey(li => li.TimeEntryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}