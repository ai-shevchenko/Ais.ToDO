using AutoMapper;

using Microsoft.Extensions.Logging.Abstractions;

namespace Ais.ToDo.Tests;

public sealed class AutoMapperTests
{
    [Fact]
    public void Validate_AutoMapperProfile_ShouldBeValid()
    {
        // Arrange 
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(Application.ApplicationModule).Assembly);
        }, NullLoggerFactory.Instance);

        // Act
        configuration.AssertConfigurationIsValid();
        
        // Assert
    }
}