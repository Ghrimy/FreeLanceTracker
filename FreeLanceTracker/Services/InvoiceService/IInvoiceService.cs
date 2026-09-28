using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.InvoiceService;

/// <summary>
/// Provides functionality to manage, retrieve, and manipulate invoice-related data.
/// </summary>
public interface IInvoiceService
{
    Task<Invoice?> GetByIdAsync(int invoiceId);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(int clientId);
    Task UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus);
    Task<decimal> GetTotalAsync(int invoiceId); 
    
    Task<InvoiceLineItem> AddLineItemAsync(InvoiceLineItem lineItem, int invoiceId);
    Task DeleteLineItemAsync(int invoiceLineItemId);
    Task UpdateLineItemAsync(InvoiceLineItem lineItem, int invoiceLineItemId);
    
    //Create invoice from unbilled time entries
    Task<Invoice> GenerateInvoiceFromUnbilledTimeAsync(int clientId, IEnumerable<int> projectIds, DateTime dueDate);
}