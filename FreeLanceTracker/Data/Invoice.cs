using System.ComponentModel.DataAnnotations;

namespace FreeLanceTracker.Data;

public enum InvoiceStatus { Draft, Sent, Paid, Overdue }
public class Invoice
{
    //properties
    [Key] public int InvoiceId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    
    //relationships
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}