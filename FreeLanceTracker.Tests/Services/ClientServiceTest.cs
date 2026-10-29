using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.Middleware;
using FreeLanceTracker.Services.ClientService;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace FreeLanceTracker.Tests.Services;

public class ClientServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WhenUserExists_SavesClient()
    {
        // Arrange
        await using var context = CreateContext();

        const string userId = "test-user-1";

        context.Users.Add(new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        });

        await context.SaveChangesAsync();

        var mapper = new Mock<IMapper>();
        var service = new ClientService(context, mapper.Object);

        var dto = new CreateClientDto
        {
            Name = "Test Client",
            Email = "client@example.com",
            Company = "Test Company",
            Description = "A test client"
        };

        // Act
        var result = await service.CreateAsync(dto, userId);

        // Assert
        Assert.True(result.ClientId > 0);
        Assert.Equal("Test Client", result.Name);
        Assert.Equal("client@example.com", result.Email);
        Assert.Equal("Test Company", result.Company);
        Assert.Equal("A test client", result.Description);
        Assert.Equal(userId, result.UserId);

        var savedClient = await context.Clients
            .SingleAsync(c => c.ClientId == result.ClientId);

        Assert.Equal("Test Client", savedClient.Name);
    }

    [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();

        var mapper = new Mock<IMapper>();
        var service = new ClientService(context, mapper.Object);

        var dto = new CreateClientDto
        {
            Name = "Test Client",
            Email = "client@example.com",
            Company = "Test Company",
            Description = "A test client"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(dto, "missing-user"));

        Assert.Equal("User not found", exception.Message);

        Assert.Empty(await context.Clients.ToListAsync());
    }
    
    [Fact]
    public async Task GetAllForUserAsync_ReturnsOnlyClientsBelongingToUser()
    {
        // Arrange
        await using var context = CreateContext();

        context.Clients.AddRange(
            new Client
            {
                Name = "User One Client",
                Email = "one@example.com",
                UserId = "user-1"
            },
            new Client
            {
                Name = "User Two Client",
                Email = "two@example.com",
                UserId = "user-2"
            });

        await context.SaveChangesAsync();

        var mapper = new Mock<IMapper>();

        List<Client>? clientsPassedToMapper = null;

        mapper
            .Setup(m => m.Map<
                IEnumerable<GetAllClientDto>>(
                It.IsAny<object>()))
            .Callback<object>(source =>
            {
                clientsPassedToMapper =
                    Assert.IsAssignableFrom<
                            IEnumerable<Client>>(source)
                        .ToList();
            })
            .Returns(Array.Empty<GetAllClientDto>());

        var service = new ClientService(context, mapper.Object);

        // Act
        await service.GetAllForUserAsync("user-1");

        // Assert
        Assert.NotNull(clientsPassedToMapper);
        var clients = clientsPassedToMapper!;

        var client = Assert.Single(clients);
        Assert.Equal("User One Client", client.Name);
        Assert.Equal("user-1", client.UserId);
    }
    
    
[Fact]
public async Task UpdateAsync_WhenClientBelongsToUser_UpdatesClient()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Old Name",
        Email = "old@example.com",
        Company = "Old Company",
        Description = "Old description",
        UserId = "user-1"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    var dto = new UpdateClientDto
    {
        ClientId = client.ClientId,
        Name = "New Name",
        Email = "new@example.com",
        Company = "New Company",
        Description = "New description"
    };

    // Act
    await service.UpdateAsync(dto, "user-1");

    // Assert
    var updatedClient = await context.Clients
        .SingleAsync(c => c.ClientId == client.ClientId);

    Assert.Equal("New Name", updatedClient.Name);
    Assert.Equal("new@example.com", updatedClient.Email);
    Assert.Equal("New Company", updatedClient.Company);
    Assert.Equal("New description", updatedClient.Description);
}

[Fact]
public async Task UpdateAsync_WhenClientBelongsToAnotherUser_ThrowsNotFoundException()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Original Name",
        Email = "original@example.com",
        UserId = "user-2"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    var dto = new UpdateClientDto
    {
        ClientId = client.ClientId,
        Name = "Hacked Name",
        Email = "hacked@example.com",
        Company = "Hacked Company",
        Description = "Hacked description"
    };

    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => service.UpdateAsync(dto, "user-1"));

    var unchangedClient = await context.Clients
        .SingleAsync(c => c.ClientId == client.ClientId);

    Assert.Equal("Original Name", unchangedClient.Name);
    Assert.Equal("original@example.com", unchangedClient.Email);
}

[Fact]
public async Task UpdateAsync_WhenClientDoesNotExist_ThrowsNotFoundException()
{
    // Arrange
    await using var context = CreateContext();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    var dto = new UpdateClientDto
    {
        ClientId = 999,
        Name = "Test Client",
        Email = "test@example.com",
        Company = "Test Company",
        Description = "Test description"
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<NotFoundException>(
        () => service.UpdateAsync(dto, "user-1"));

    Assert.Equal("Client not found", exception.Message);
}

[Fact]
public async Task ArchiveAsync_WhenClientBelongsToUser_ArchivesClient()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Test Client",
        Email = "test@example.com",
        UserId = "user-1",
        IsArchived = false
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    // Act
    await service.ArchiveAsync(client.ClientId, "user-1");

    // Assert
    var archivedClient = await context.Clients
        .SingleAsync(c => c.ClientId == client.ClientId);

    Assert.True(archivedClient.IsArchived);
}

[Fact]
public async Task ArchiveAsync_WhenClientBelongsToAnotherUser_ThrowsNotFoundException()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Test Client",
        Email = "test@example.com",
        UserId = "user-2",
        IsArchived = false
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => service.ArchiveAsync(client.ClientId, "user-1"));

    var unchangedClient = await context.Clients
        .SingleAsync(c => c.ClientId == client.ClientId);

    Assert.False(unchangedClient.IsArchived);
}

[Fact]
public async Task DeleteAsync_WhenClientHasNoProjectsOrInvoices_DeletesClient()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Test Client",
        Email = "test@example.com",
        UserId = "user-1"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var clientId = client.ClientId;

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    // Act
    await service.DeleteAsync(clientId, "user-1");

    // Assert
    var deletedClient = await context.Clients
        .SingleOrDefaultAsync(c => c.ClientId == clientId);

    Assert.Null(deletedClient);
}


[Fact]
public async Task DeleteAsync_WhenClientHasProjects_ThrowsValidationException()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Test Client",
        Email = "test@example.com",
        UserId = "user-1"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    context.Projects.Add(new Project
    {
        Name = "Test Project",
        ClientId = client.ClientId,
        Status = ProjectStatus.Active,
        HourlyRate = 50m,
        StartDate = new DateTime(2026, 1, 1)
    });

    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    // Act & Assert
    var exception =
        await Assert.ThrowsAsync<
            System.ComponentModel.DataAnnotations.ValidationException>(
            () => service.DeleteAsync(client.ClientId, "user-1"));

    Assert.Contains("cannot be deleted", exception.Message);

    Assert.True(await context.Clients
        .AnyAsync(c => c.ClientId == client.ClientId));
}



[Fact]
public async Task DeleteAsync_WhenClientHasInvoices_ThrowsValidationException()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Test Client",
        Email = "test@example.com",
        UserId = "user-1"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    context.Invoices.Add(new Invoice
    {
        InvoiceNumber = "INV-001",
        ClientId = client.ClientId,
        IssueDate = new DateTime(2026, 1, 1),
        DueDate = new DateTime(2026, 2, 1),
        Status = InvoiceStatus.Draft
    });

    await context.SaveChangesAsync();

    var service = new ClientService(
        context,
        new Mock<IMapper>().Object);

    // Act & Assert
    var exception =
        await Assert.ThrowsAsync<
            System.ComponentModel.DataAnnotations.ValidationException>(
            () => service.DeleteAsync(client.ClientId, "user-1"));

    Assert.Contains("cannot be deleted", exception.Message);

    Assert.True(await context.Clients
        .AnyAsync(c => c.ClientId == client.ClientId));
}

[Fact]
public async Task GetClientIdAsync_WhenClientBelongsToUser_MapsCorrectClient()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "My Client",
        Email = "client@example.com",
        UserId = "user-1"
    };

    context.Clients.AddRange(
        client,
        new Client
        {
            Name = "Other Client",
            Email = "other@example.com",
            UserId = "user-2"
        });

    await context.SaveChangesAsync();

    Client? mappedClient = null;

    var mapper = new Mock<IMapper>();

    mapper
        .Setup(m => m.Map<ClientDto>(It.IsAny<object>()))
        .Callback<object>(source =>
            mappedClient = Assert.IsType<Client>(source))
        .Returns((ClientDto)null!);

    var service = new ClientService(context, mapper.Object);

    // Act
    await service.GetClientIdAsync(client.ClientId, "user-1");

    // Assert
    Assert.NotNull(mappedClient);
    Assert.Equal(client.ClientId, mappedClient.ClientId);
    Assert.Equal("user-1", mappedClient.UserId);
    Assert.Equal("My Client", mappedClient.Name);
}

[Fact]
public async Task GetClientIdAsync_WhenClientBelongsToAnotherUser_ThrowsNotFoundException()
{
    // Arrange
    await using var context = CreateContext();

    var client = new Client
    {
        Name = "Other Client",
        Email = "other@example.com",
        UserId = "user-2"
    };

    context.Clients.Add(client);
    await context.SaveChangesAsync();

    var mapper = new Mock<IMapper>();
    var service = new ClientService(context, mapper.Object);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<NotFoundException>(
        () => service.GetClientIdAsync(client.ClientId, "user-1"));

    Assert.Equal("Client not found", exception.Message);

    mapper.Verify(
        m => m.Map<ClientDto>(It.IsAny<object>()),
        Times.Never);
}


}
