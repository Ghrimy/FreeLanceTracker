using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.InvoiceDtos;
using FreeLanceTracker.DTOs.LineItemDto;
using FreeLanceTracker.Middleware;
using Microsoft.EntityFrameworkCore;

namespace FreeLanceTracker.Services.InvoiceService;

public class InvoiceService(ApplicationDbContext context, IMapper mapper) : IInvoiceService
{
    public async Task<InvoiceDto> CreateDraftAsync(int clientId, DateTime dueDate, string userId)
    {
        await ValidateClientAndDueDateAsync(clientId, dueDate, userId);
        var invoice = new Invoice
        {
            ClientId = clientId,
            IssueDate = DateTime.UtcNow,
            DueDate = dueDate.Date,
            Status = InvoiceStatus.Draft,
            InvoiceNumber = await GenerateInvoiceNumberAsync()
        };
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();
        return mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<Invoice> GetByIdAsync(int invoiceId, string userId)
    {
        var invoice = await context.Invoices.Include(i => i.LineItems)
            .Where(i => i.InvoiceId == invoiceId && i.Client != null && i.Client.UserId == userId)
            .FirstOrDefaultAsync();

        if (invoice is null) throw new NotFoundException("Invoice not found");


        return invoice;
    }

    public async Task<IEnumerable<InvoiceDto>> GetByClientIdAsync(int clientId, string userId)
    {
        var invoices = await context.Invoices.Include(i => i.LineItems)
            .Where(i => i.ClientId == clientId && i.Client != null && i.Client.UserId == userId).ToListAsync();

        if (invoices is null) throw new NotFoundException("Invoices not found");
        var dtos = mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return dtos;
    }

    public async Task UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus, string userId)
    {
        if (!Enum.IsDefined(newStatus)) throw new ArgumentException("Invalid invoice status.");
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

    public async Task<InvoiceLineItemDto> AddLineItemAsync(InvoiceLineItemDto lineItem, int invoiceId, string userId)
    {
        var invoice = await GetByIdAsync(invoiceId, userId);
        if (invoice is null) throw new NotFoundException("Invoice not found");

        if (invoice.Status != InvoiceStatus.Draft)
            throw new InvoiceLockedException("Only draft invoices can have their items changed.");

        await ValidateLineItemAsync(lineItem, invoice, userId);
        var addLineItem = new InvoiceLineItem
        {
            InvoiceId = invoiceId,
            Description = lineItem.Description.Trim(),
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice,
            ProjectId = lineItem.ProjectId,
            TimeEntryId = lineItem.TimeEntryId
        };

        invoice.LineItems.Add(addLineItem);

        await context.SaveChangesAsync();

        return new InvoiceLineItemDto
        {
            InvoiceLineItemId = addLineItem.InvoiceLineItemId,
            InvoiceId = addLineItem.InvoiceId,
            Description = addLineItem.Description,
            Quantity = addLineItem.Quantity,
            UnitPrice = addLineItem.UnitPrice,
            ProjectId = addLineItem.ProjectId,
            TimeEntryId = addLineItem.TimeEntryId
        };
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

        if (lineItem.Invoice != null && lineItem.Invoice.Status != InvoiceStatus.Draft)
            throw new InvoiceLockedException("Only draft invoices can have their items changed.");

        context.InvoiceLineItems.Remove(lineItem);
        await context.SaveChangesAsync();
    }

    public async Task UpdateLineItemAsync(InvoiceLineItemDto lineItem, int invoiceLineItemId, string userId)
    {
        var existing = await context.InvoiceLineItems
            .Where(i => i.InvoiceLineItemId == invoiceLineItemId
                        && i.Invoice != null
                        && i.Invoice.Client != null
                        && i.Invoice.Client.UserId == userId)
            .Include(invoiceLineItem => invoiceLineItem.Invoice).FirstOrDefaultAsync();
        if (existing is null)
            throw new NotFoundException("Invoice line item not found");

        if (existing.Invoice != null && existing.Invoice.Status != InvoiceStatus.Draft)
            throw new InvoiceLockedException("Only draft invoices can have their items changed.");

        await ValidateLineItemAsync(lineItem, existing.Invoice!, userId);
        existing.Description = lineItem.Description.Trim();
        existing.Quantity = lineItem.Quantity;
        existing.UnitPrice = lineItem.UnitPrice;
        existing.ProjectId = lineItem.ProjectId;
        existing.TimeEntryId = lineItem.TimeEntryId;

        await context.SaveChangesAsync();
    }

    // Generates an invoice for unbilled time entries associated with the specified client and projects.
    public async Task<InvoiceDto> GenerateInvoiceFromUnbilledTimeAsync(int clientId, IEnumerable<int> projectIds,
        DateTime dueDate, string userId)
    {
        await ValidateClientAndDueDateAsync(clientId, dueDate, userId);
        var selectedProjectIds = projectIds?.Distinct().ToArray() ?? [];
        if (selectedProjectIds.Length == 0 || await context.Projects.CountAsync(p =>
                selectedProjectIds.Contains(p.ProjectId) && p.ClientId == clientId
                                                         && p.Client != null && p.Client.UserId == userId) !=
            selectedProjectIds.Length)
            throw new ArgumentException("Select projects belonging to this client.");

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Get all unbilled time entries for the specified client and projects
            var unbilledInvoices = await context.TimeEntries
                .Include(p => p.Project)
                .Where(t => selectedProjectIds.Contains(t.ProjectId)
                            && t.IsBillable
                            && !t.IsBilled
                            && t.Project != null
                            && t.Project.Client != null
                            && t.Project.ClientId == clientId
                            && t.Project.Client.UserId == userId)
                .ToListAsync();

            if (!unbilledInvoices.Any())
                throw new NotFoundException("No unbilled time entries found for the selected projects.");
            if (unbilledInvoices.Any(t => t.Hours <= 0 || t.Project!.HourlyRate < 0))
                throw new ArgumentException("Billable time must have positive hours and a non-negative project rate.");

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
            await transaction.CommitAsync();

            var dto = mapper.Map<InvoiceDto>(invoice);
            return dto;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task ValidateClientAndDueDateAsync(int clientId, DateTime dueDate, string userId)
    {
        if (dueDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Due date cannot be before today.");
        if (!await context.Clients.AnyAsync(c => c.ClientId == clientId && c.UserId == userId))
            throw new NotFoundException("Client not found");
    }


    private async Task ValidateLineItemAsync(InvoiceLineItemDto lineItem, Invoice invoice, string userId)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Description) || lineItem.Description.Trim().Length > 200)
            throw new ArgumentException("Description is required and must be at most 200 characters.");
        if (lineItem.Quantity <= 0 || lineItem.UnitPrice < 0)
            throw new ArgumentException("Quantity must be positive and unit price cannot be negative.");
        if (!await context.Projects.AnyAsync(p => p.ProjectId == lineItem.ProjectId
                                                  && p.ClientId == invoice.ClientId && p.Client != null &&
                                                  p.Client.UserId == userId))
            throw new ArgumentException("Select a project belonging to this invoice's client.");
        if (lineItem.TimeEntryId.HasValue && !await context.TimeEntries.AnyAsync(t =>
                t.TimeEntryId == lineItem.TimeEntryId && t.ProjectId == lineItem.ProjectId))
            throw new ArgumentException("Time entry must belong to the selected project.");
    }

    /// <summary>
    ///     TODO: Will need updating.
    ///     Generates a unique invoice number based on the current year and the count of invoices issued in that year.
    /// </summary>
    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var count = await context.Invoices.Where(i => i.IssueDate.Year == year).CountAsync();

        return $"INV-{year}-{count}";
    }
}