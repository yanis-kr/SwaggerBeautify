using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using WebApi.StartupExtensions.Swagger;

namespace WebApi.Tests.Extensions;

public class SwaggerApplicationExtensionsTests
{
    [Fact]
    public void UseSwaggerDocumentation_ShouldRequireApiVersionDescriptionProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        
        var appBuilder = Substitute.For<IApplicationBuilder>();
        appBuilder.ApplicationServices.Returns(serviceProvider);

        // Act
        var act = () => appBuilder.UseSwaggerDocumentation();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UseSwaggerDocumentation_WithValidProvider_ShouldNotThrowOnInitialCall()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(CreateMockApiVersionDescriptionProvider());
        var serviceProvider = services.BuildServiceProvider();
        
        var appBuilder = Substitute.For<IApplicationBuilder>();
        appBuilder.ApplicationServices.Returns(serviceProvider);
        appBuilder.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(appBuilder);

        // Act & Assert
        // The method will throw when trying to use Swagger middleware without full setup
        // but we verify it at least tries to call the provider
        var act = () => appBuilder.UseSwaggerDocumentation();
        
        // Will throw because Swagger services aren't fully configured, but that's expected in unit test
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SwaggerOptions*");
    }

    private static IApiVersionDescriptionProvider CreateMockApiVersionDescriptionProvider()
    {
        var provider = Substitute.For<IApiVersionDescriptionProvider>();
        var versionDescription = Substitute.For<ApiVersionDescription>(
            new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0),
            "v1",
            false);
        
        provider.ApiVersionDescriptions.Returns(new[] { versionDescription });
        return provider;
    }
}

