
using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.MappingProfile;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FreeLanceTracker.Tests.Mapping;

public class ClientMappingTests
{
    private readonly IMapper _mapper;

    public ClientMappingTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile.MappingProfile>();
        }, NullLoggerFactory.Instance);

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void UpdateClientDto_ToClient_MapsEditablePropertiesOnly()
    {
        // Arrange
        var existingClient = new Client
        {
            ClientId = 10,
            Name = "Old Name",
            Email = "old@example.com",
            Company = "Old Company",
            Description = "Old description",
            UserId = "user-123",
            IsArchived = true
        };

        var updateDto = new UpdateClientDto
        {
            ClientId = 10,
            Name = "New Name",
            Email = "new@example.com",
            Company = "New Company",
            Description = "New description"
        };

        // Act
        _mapper.Map(updateDto, existingClient);

        // Assert
        Assert.Equal(10, existingClient.ClientId);
        Assert.Equal("New Name", existingClient.Name);
        Assert.Equal("new@example.com", existingClient.Email);
        Assert.Equal("New Company", existingClient.Company);
        Assert.Equal("New description", existingClient.Description);

        // These properties must not be overwritten by the DTO.
        Assert.Equal("user-123", existingClient.UserId);
        Assert.True(existingClient.IsArchived);
    }

    [Fact]
    public void Client_ToClientDto_MapsPropertiesCorrectly()
    {
        // Arrange
        var client = new Client
        {
            ClientId = 10,
            Name = "Test Client",
            Email = "test@example.com",
            Company = "Test Company",
            Description = "Test description",
            UserId = "user-123"
        };

        // Act
        var result = _mapper.Map<ClientDto>(client);

        // Assert
        Assert.Equal(10, result.ClientId);
        Assert.Equal("Test Client", result.Name);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("Test Company", result.Company);
        Assert.Equal("Test description", result.Description);
    }
}
