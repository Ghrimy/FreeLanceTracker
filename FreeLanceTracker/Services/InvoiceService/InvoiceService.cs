using FreeLanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.InvoiceService;

public class InvoiceService(ApplicationDbContext context) : IInvoiceService
{

    public async Task<Invoice?> GetByIdAsync(int invoiceId)
    {
        var invoice = context.Invoices.Where(i => i.InvoiceId == invoiceId).FirstOrDefaultAsync();
        return await invoice;
    }

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(int clientId)
    {
        var invoices = context.Invoices.Where(i => i.ClientId == clientId).ToListAsync();
        return await invoices;
    }

    public async Task UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus)
    {
        var invoiceToUpdate = await GetByIdAsync(invoiceId);
        if (invoiceToUpdate is null)
        {
            throw new Exception("Invoice not found");
        }
        invoiceToUpdate.Status = newStatus;
        context.Invoices.Update(invoiceToUpdate);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalAsync(int invoiceId)
    {
        throw new NotImplementedException();
    }

    public async Task<InvoiceLineItem> AddLineItemAsync(InvoiceLineItem lineItem, int invoiceId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteLineItemAsync(int invoiceLineItemId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateLineItemAsync(InvoiceLineItem lineItem, int invoiceLineItemId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates an invoice for unbilled time entries associated with the specified client and projects.
    /// </summary>
    /// <param name="clientId">The ID of the client for whom the invoice is being generated.</param>
    /// <param name="projectIds">A collection of project IDs for which unbilled time entries should be included in the invoice.</param>
    /// <param name="dueDate">The due date for the invoice.</param>
    /// <returns>The newly created <see cref="Invoice"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if there are no billable time entries for the provided client and projects.</exception>
    public async Task<Invoice> GenerateInvoiceFromUnbilledTimeAsync(int clientId, IEnumerable<int> projectIds,
        DateTime dueDate)
    {
        // Get all unbilled time entries for the specified client and projects
        var unbilledInvoices = await context.TimeEntries.Include(p => p.Project)
            .Where(t => projectIds.Contains(t.ProjectId)
                        && t.IsBillable && !t.IsBilled && t.Project.ClientId == clientId)
            .ToListAsync();

        if (unbilledInvoices is null)
            throw new InvalidOperationException("There are no current invoices to generated.");

        // Create a new invoice
        var invoice = new Invoice
        {
            ClientId = clientId,
            IssueDate = DateTime.UtcNow,
            DueDate = dueDate,
            Status = InvoiceStatus.Draft,
            InvoiceNumber = await GenerateInvoiceNumberAsync(),
            LineItems = new List<InvoiceLineItem>()
        };

        var groupedByProject = unbilledInvoices.GroupBy(t => t.Project);

        // Calculate total hours for each project and adds it as one item instead of individual items
        foreach (var group in groupedByProject)
        {
            var project = group.Key;
            var totalHours = group.Sum(t => t.Hours);

            invoice.LineItems.Add(new InvoiceLineItem
            {
                Description = $"{project.Name} — {totalHours}h",
                Quantity = totalHours,
                UnitPrice = project.HourlyRate,
                ProjectId = project.ProjectId
            });
        }

        // Mark all time entries as billed
        foreach (var entry in unbilledInvoices)
            entry.IsBilled = true;

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return invoice;
    }
    
    /// <summary>
    /// Generates a unique invoice number based on the current year and the count of invoices issued in that year.
    /// </summary>

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var countThisYear = await context.Invoices
            .CountAsync(i => i.IssueDate.Year == year);

        return $"INV-{year}-{(countThisYear + 1):D4}";
    }
}