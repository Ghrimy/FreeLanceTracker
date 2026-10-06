using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.Data;

namespace FreeLanceTracker.DTOs.InvoiceDtos;

public class UpdateInvoiceStatusDto
{
    [Required] public int InvoiceId { get; set; }
    [Required] public InvoiceStatus Status { get; set; }
}