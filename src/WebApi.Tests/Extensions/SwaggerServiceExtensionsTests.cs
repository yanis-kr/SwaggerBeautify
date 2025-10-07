using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger;

namespace WebApi.Tests.Extensions;

public class SwaggerServiceExtensionsTests
{
    [Fact]
    public void AddSwaggerDocumentation_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddSwaggerDocumentation();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldRegisterSwaggerGenOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerDocumentation();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>));
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldAddMultipleServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerDocumentation();

        // Assert
        services.Should().NotBeEmpty();
        services.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSwaggerDocumentation();

        // Assert
        act.Should().NotThrow();
    }
}

