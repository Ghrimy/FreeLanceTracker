using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.InvoiceService;

/// <summary>
/// Provides functionality to manage, retrieve, and manipulate invoice-related data.
/// </summary>
public interface IInvoiceService
{
    Task<Invoice?> GetByIdAsync(int invoiceId, string userId);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(int clientId, string userId);
    Task UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus, string userId);
    Task<decimal> GetTotalAsync(int invoiceId, string userId); 
    
    Task<InvoiceLineItem> AddLineItemAsync(InvoiceLineItem lineItem, int invoiceId, string userId);
    Task DeleteLineItemAsync(int invoiceLineItemId, string userId);
    Task UpdateLineItemAsync(InvoiceLineItem lineItem, int invoiceLineItemId, string userId);
    
    //Create invoice from unbilled time entries
    Task<Invoice> GenerateInvoiceFromUnbilledTimeAsync(int clientId, IEnumerable<int> projectIds, DateTime dueDate, string userId);
}