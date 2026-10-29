using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace FreeLanceTracker.Tests.Services;


public class MappingProfileTests
{
    [Fact]
    public void MappingProfile_ConfigurationIsValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile.MappingProfile>();
        }, NullLoggerFactory.Instance);

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }
}