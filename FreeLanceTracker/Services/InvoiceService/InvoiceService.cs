using FreeLanceTracker.Data;
using FreeLanceTracker.Middleware;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.InvoiceService;

public class InvoiceService(ApplicationDbContext context) : IInvoiceService
{
    public async Task<Invoice?> GetByIdAsync(int invoiceId, string userId)
    {
        var invoice = context.Invoices.Include(i => i.LineItems)
            .Where(i => i.InvoiceId == invoiceId && i.Client != null && i.Client.UserId == userId)
            .FirstOrDefaultAsync();
        return await invoice;
    }

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(int clientId, string userId)
    {
        var invoices = context.Invoices
            .Where(i => i.ClientId == clientId && i.Client != null && i.Client.UserId == userId).ToListAsync();
        return await invoices;
    }

    public async Task UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus, string userId)
    {
        var invoice = await GetByIdAsync(invoiceId, userId);
        if (invoice is null) throw new NotFoundException("Invoice not found");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvoiceLockedException("Cannot change the status of a paid invoice.");

        invoice.Status = newStatus;
        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalAsync(int invoiceId, string userId)
    {
        var invoice = await GetByIdAsync(invoiceId, userId);
        if (invoice is null) throw new NotFoundException("Invoice not found");
        return invoice.LineItems.Sum(i => i.Quantity * i.UnitPrice);
    }

    public async Task<InvoiceLineItem> AddLineItemAsync(InvoiceLineItem lineItem, int invoiceId, string userId)
    {
        var invoice = await GetByIdAsync(invoiceId, userId);
        if (invoice is null) throw new NotFoundException("Invoice not found");
        invoice.LineItems.Add(lineItem);
        context.Invoices.Update(invoice);
        await context.SaveChangesAsync();
        return lineItem;
    }

    public async Task DeleteLineItemAsync(int invoiceLineItemId, string userId)
    {
        var lineItem = await context.InvoiceLineItems
            .Include(li => li.Invoice)
            .Where(li => li.InvoiceLineItemId == invoiceLineItemId
                         && li.Invoice != null
                         && li.Invoice.Client != null
                         && li.Invoice.Client.UserId == userId)
            .FirstOrDefaultAsync();
        if (lineItem is null) throw new NotFoundException("Invoice line item not found");
        context.InvoiceLineItems.Remove(lineItem);
        await context.SaveChangesAsync();
    }

    public async Task UpdateLineItemAsync(InvoiceLineItem lineItem, int invoiceLineItemId, string userId)
    {
        var existing = await context.InvoiceLineItems.Where(i => i.InvoiceLineItemId == invoiceLineItemId
                                                                 && i.Project != null
                                                                 && i.Project.Client != null
                                                                 && i.Project.Client.UserId == userId)
            .Include(invoiceLineItem => invoiceLineItem.Invoice).FirstOrDefaultAsync();
        if (existing is null)
            throw new NotFoundException("Invoice line item not found");

        if (existing.Invoice != null && existing.Invoice.Status != InvoiceStatus.Draft)
            throw new InvoiceLockedException("Cannot edit a line item on a paid invoice.");

        existing.Description = lineItem.Description;
        existing.Quantity = lineItem.Quantity;
        existing.UnitPrice = lineItem.UnitPrice;
        existing.ProjectId = lineItem.ProjectId;
        existing.TimeEntryId = lineItem.TimeEntryId;

        await context.SaveChangesAsync();
    }


    // Generates an invoice for unbilled time entries associated with the specified client and projects.
    public async Task<Invoice> GenerateInvoiceFromUnbilledTimeAsync(int clientId, IEnumerable<int> projectIds,
        DateTime dueDate, string userId)
    {
        // Get all unbilled time entries for the specified client and projects
        var unbilledInvoices = await context.TimeEntries.Include(p => p.Project)
            .Where(t => projectIds.Contains(t.ProjectId)
                        && t.IsBillable
                        && !t.IsBilled
                        && t.Project != null
                        && t.Project.Client != null
                        && t.Project.ClientId == clientId
                        && t.Project.Client.UserId == userId)
            .ToListAsync();

        if (!unbilledInvoices.Any())
            throw new NotFoundException("No unbilled time entries found for the selected projects.");

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
    ///     Generates a unique invoice number based on the current year and the count of invoices issued in that year.
    /// </summary>
    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var countThisYear = await context.Invoices
            .CountAsync(i => i.IssueDate.Year == year);

        return $"INV-{year}-{countThisYear + 1:D4}";
    }
}