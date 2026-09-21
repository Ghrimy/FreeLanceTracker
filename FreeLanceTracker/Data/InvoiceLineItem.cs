using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.Data;

public class InvoiceLineItem
{
    //properties
    [Key] public int InvoiceLineItemId { get; set; }
    [StringLength(200)] public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    //relationships
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int? ProjectId { get; set; } 
    public Project? Project { get; set; }
    
    public int? TimeEntryId { get; set; }
    public TimeEntry? TimeEntry { get; set; }

}