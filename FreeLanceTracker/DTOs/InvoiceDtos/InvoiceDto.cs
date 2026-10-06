using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.Data;

namespace FreeLanceTracker.DTOs.InvoiceDtos;

public class InvoiceDto
{
    [Required] public int InvoiceId { get; set; }
    [Required] public string InvoiceNumber { get; set; } = string.Empty;
    [Required] public DateTime IssueDate { get; set; }
    [Required] public DateTime DueDate { get; set; }
    [Required] public InvoiceStatus Status { get; set; }
    [Required] public int ClientId { get; set; }
    [Required] public IEnumerable<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

}