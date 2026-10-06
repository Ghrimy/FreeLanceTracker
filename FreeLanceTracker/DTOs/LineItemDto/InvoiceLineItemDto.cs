using System.ComponentModel.DataAnnotations;
using FreeLanceTracker.DTOs.ProjectDtos;
using FreeLanceTracker.DTOs.TimeEntryDtos;
using FreeLanceTracker.DTOs.InvoiceDtos;

namespace FreeLanceTracker.DTOs.LineItemDto;

public class InvoiceLineItemDto
{
    //properties
    [Required] public int InvoiceLineItemId { get; set; }
    [Required] [StringLength(200)] public string Description { get; set; } = string.Empty;
    [Required] public decimal Quantity { get; set; }
    [Required] public decimal UnitPrice { get; set; }

    //relationships
    [Required] public int InvoiceId { get; set; }
    [Required] public InvoiceDto Invoice { get; set; }

    [Required] public int? ProjectId { get; set; }
    [Required] public ProjectDto? Project { get; set; }

    [Required]public int? TimeEntryId { get; set; }
    [Required] public TimeEntryDto TimeEntry { get; set; }
}