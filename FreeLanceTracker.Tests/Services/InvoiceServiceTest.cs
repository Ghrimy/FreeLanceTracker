
using System;
using System.Threading.Tasks;
using FreeLanceTracker.Data;
using FreeLanceTracker.Middleware;
using FreeLanceTracker.Services.InvoiceService;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using FreeLanceTracker.DTOs.LineItemDto;
using Xunit;

namespace FreeLanceTracker.Tests.Services;

public class InvoiceServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static async Task<(Client client, Invoice invoice)> SeedInvoiceAsync(
        ApplicationDbContext context,
        string userId,
        InvoiceStatus status = InvoiceStatus.Draft)
    {
        var client = new Client
        {
            Name = "Test Client",
            Email = "client@example.com",
            UserId = userId
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.ClientId,
            InvoiceNumber = "INV-TEST-001",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = status
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return (client, invoice);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInvoiceBelongsToUser_ReturnsInvoice()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-1");

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act
        var result = await service.GetByIdAsync(invoice.InvoiceId, "user-1");

        // Assert
        Assert.Equal(invoice.InvoiceId, result.InvoiceId);
        Assert.Equal("INV-TEST-001", result.InvoiceNumber);
        Assert.Equal(InvoiceStatus.Draft, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInvoiceBelongsToAnotherUser_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-2");

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(invoice.InvoiceId, "user-1"));

        Assert.Equal("Invoice not found", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenInvoiceIsDraft_UpdatesStatus()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-1");

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act
        await service.UpdateStatusAsync(
            invoice.InvoiceId,
            InvoiceStatus.Sent,
            "user-1");

        // Assert
        var updatedInvoice = await context.Invoices.FindAsync(invoice.InvoiceId);

        Assert.NotNull(updatedInvoice);
        Assert.Equal(InvoiceStatus.Sent, updatedInvoice.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenInvoiceIsPaid_ThrowsInvoiceLockedException()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(
            context,
            "user-1",
            InvoiceStatus.Paid);

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvoiceLockedException>(
            () => service.UpdateStatusAsync(
                invoice.InvoiceId,
                InvoiceStatus.Sent,
                "user-1"));

        Assert.Equal(
            "Cannot change the status of a paid invoice.",
            exception.Message);
    }
    
    [Fact]
    public async Task GetTotalAsync_WithMultipleLineItems_ReturnsCorrectTotal()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-1");

        context.InvoiceLineItems.AddRange(
            new InvoiceLineItem
            {
                InvoiceId = invoice.InvoiceId,
                Description = "Development",
                Quantity = 5,
                UnitPrice = 50
            },
            new InvoiceLineItem
            {
                InvoiceId = invoice.InvoiceId,
                Description = "Consulting",
                Quantity = 2,
                UnitPrice = 75
            });

        await context.SaveChangesAsync();

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act
        var total = await service.GetTotalAsync(invoice.InvoiceId, "user-1");

        // Assert: (5 × 50) + (2 × 75) = 400
        Assert.Equal(400m, total);
    }
    
    [Fact]
    public async Task AddLineItemAsync_WhenInvoiceIsDraft_AddsLineItem()
    {
        // Arrange
        await using var context = CreateContext();
        var (client, invoice) = await SeedInvoiceAsync(context, "user-1");

        var project = new Project
        {
            Name = "Development Project",
            ClientId = client.ClientId,
            HourlyRate = 50,
            StartDate = DateTime.UtcNow
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var lineItemDto = new InvoiceLineItemDto
        {
            Description = "Development work",
            Quantity = 3,
            UnitPrice = 50,
            ProjectId = project.ProjectId
        };

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act
        var result = await service.AddLineItemAsync(
            lineItemDto,
            invoice.InvoiceId,
            "user-1");

        // Assert
        Assert.True(result.InvoiceLineItemId > 0);
        Assert.Equal("Development work", result.Description);
        Assert.Equal(3, result.Quantity);
        Assert.Equal(50, result.UnitPrice);

        var savedItem = await context.InvoiceLineItems
            .FirstOrDefaultAsync(i =>
                i.InvoiceLineItemId == result.InvoiceLineItemId);

        Assert.NotNull(savedItem);
        Assert.Equal(invoice.InvoiceId, savedItem.InvoiceId);
    }
    
    [Fact]
    public async Task AddLineItemAsync_WhenInvoiceIsSent_ThrowsInvoiceLockedException()
    {
        // Arrange
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(
            context,
            "user-1",
            InvoiceStatus.Sent);

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var lineItem = new InvoiceLineItemDto
        {
            Description = "Development work",
            Quantity = 2,
            UnitPrice = 50
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvoiceLockedException>(
            () => service.AddLineItemAsync(
                lineItem,
                invoice.InvoiceId,
                "user-1"));

        Assert.Equal(
            "Only draft invoices can have their items changed.",
            exception.Message);
    }


    [Fact]
    public async Task AddLineItemAsync_WhenQuantityIsZero_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateContext();
        var (client, invoice) = await SeedInvoiceAsync(context, "user-1");

        var project = new Project
        {
            Name = "Development Project",
            ClientId = client.ClientId,
            HourlyRate = 50,
            StartDate = DateTime.UtcNow
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var lineItem = new InvoiceLineItemDto
        {
            Description = "Development work",
            Quantity = 0,
            UnitPrice = 50,
            ProjectId = project.ProjectId
        };

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddLineItemAsync(
                lineItem,
                invoice.InvoiceId,
                "user-1"));

        Assert.Contains("Quantity must be positive", exception.Message);

        Assert.Empty(await context.InvoiceLineItems.ToListAsync());
    }
    
    
    [Fact]
    public async Task UpdateLineItemAsync_WhenInvoiceIsDraft_UpdatesLineItem()
    {
        await using var context = CreateContext();
        var (client, invoice) = await SeedInvoiceAsync(context, "user-1");

        var project = new Project
        {
            Name = "Development Project",
            ClientId = client.ClientId,
            HourlyRate = 50,
            StartDate = DateTime.UtcNow
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var item = new InvoiceLineItem
        {
            InvoiceId = invoice.InvoiceId,
            Description = "Old description",
            Quantity = 1,
            UnitPrice = 25,
            ProjectId = project.ProjectId
        };

        context.InvoiceLineItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var updatedDto = new InvoiceLineItemDto
        {
            Description = "Updated development work",
            Quantity = 3,
            UnitPrice = 50,
            ProjectId = project.ProjectId
        };

        await service.UpdateLineItemAsync(
            updatedDto,
            item.InvoiceLineItemId,
            "user-1");

        var updatedItem = await context.InvoiceLineItems
            .FindAsync(item.InvoiceLineItemId);

        Assert.NotNull(updatedItem);
        Assert.Equal("Updated development work", updatedItem.Description);
        Assert.Equal(3, updatedItem.Quantity);
        Assert.Equal(50, updatedItem.UnitPrice);
    }

    
    
    [Fact]
    public async Task UpdateLineItemAsync_WhenInvoiceIsSent_ThrowsInvoiceLockedException()
    {
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(
            context, "user-1", InvoiceStatus.Sent);

        var item = new InvoiceLineItem
        {
            InvoiceId = invoice.InvoiceId,
            Description = "Existing work",
            Quantity = 2,
            UnitPrice = 50
        };

        context.InvoiceLineItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var updatedDto = new InvoiceLineItemDto
        {
            Description = "Changed work",
            Quantity = 3,
            UnitPrice = 60
        };

        var exception = await Assert.ThrowsAsync<InvoiceLockedException>(
            () => service.UpdateLineItemAsync(
                updatedDto,
                item.InvoiceLineItemId,
                "user-1"));

        Assert.Equal(
            "Only draft invoices can have their items changed.",
            exception.Message);
    }
    
    
    [Fact]
    public async Task DeleteLineItemAsync_WhenInvoiceIsDraft_DeletesLineItem()
    {
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-1");

        var item = new InvoiceLineItem
        {
            InvoiceId = invoice.InvoiceId,
            Description = "Work to remove",
            Quantity = 2,
            UnitPrice = 50
        };

        context.InvoiceLineItems.Add(item);
        await context.SaveChangesAsync();

        var itemId = item.InvoiceLineItemId;
        var service = new InvoiceService(context, Mock.Of<IMapper>());

        await service.DeleteLineItemAsync(itemId, "user-1");

        var deletedItem = await context.InvoiceLineItems
            .FindAsync(itemId);

        Assert.Null(deletedItem);
    }
    
    
    [Fact]
    public async Task DeleteLineItemAsync_WhenItemBelongsToAnotherUser_ThrowsNotFoundException()
    {
        await using var context = CreateContext();
        var (_, invoice) = await SeedInvoiceAsync(context, "user-2");

        var item = new InvoiceLineItem
        {
            InvoiceId = invoice.InvoiceId,
            Description = "Private work",
            Quantity = 1,
            UnitPrice = 100
        };

        context.InvoiceLineItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteLineItemAsync(
                item.InvoiceLineItemId,
                "user-1"));

        Assert.Equal("Invoice line item not found", exception.Message);

        Assert.True(
            await context.InvoiceLineItems.AnyAsync(
                i => i.InvoiceLineItemId == item.InvoiceLineItemId));
    }
    
    [Fact]
    public async Task CreateDraftAsync_WhenDueDateIsInPast_ThrowsArgumentException()
    {
        await using var context = CreateContext();
        var (client, _) = await SeedInvoiceAsync(context, "user-1");

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateDraftAsync(
                client.ClientId,
                DateTime.UtcNow.Date.AddDays(-1),
                "user-1"));

        Assert.Equal(
            "Due date cannot be before today.",
            exception.Message);
    }

    
    [Fact]
    public async Task CreateDraftAsync_WhenClientBelongsToAnotherUser_ThrowsNotFoundException()
    {
        await using var context = CreateContext();
        var (client, _) = await SeedInvoiceAsync(context, "user-2");

        var service = new InvoiceService(context, Mock.Of<IMapper>());

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateDraftAsync(
                client.ClientId,
                DateTime.UtcNow.Date.AddDays(14),
                "user-1"));

        Assert.Equal("Client not found", exception.Message);
    }


}
