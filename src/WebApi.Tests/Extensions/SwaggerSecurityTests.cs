using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger;

namespace WebApi.Tests.Extensions;

public class SwaggerSecurityTests
{
    private readonly SwaggerGenOptions _options;

    public SwaggerSecurityTests()
    {
        var services = new ServiceCollection();
        services.AddSwaggerDocumentation();
        var serviceProvider = services.BuildServiceProvider();
        
        var optionsSnapshot = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        _options = new SwaggerGenOptions();
        optionsSnapshot?.Configure(_options);
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldAddBearerSecurityDefinition()
    {
        // Assert
        _options.SwaggerGeneratorOptions.SecuritySchemes.Should().ContainKey("Bearer");
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveCorrectType()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Type.Should().Be(SecuritySchemeType.Http);
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveCorrectScheme()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Scheme.Should().Be("bearer");
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveJWTFormat()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.BearerFormat.Should().Be("JWT");
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldBeInHeader()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.In.Should().Be(ParameterLocation.Header);
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveAuthorizationName()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Name.Should().Be("Authorization");
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveDescription()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Description.Should().NotBeNullOrEmpty();
        bearerScheme.Description.Should().Contain("JWT");
        bearerScheme.Description.Should().Contain("Bearer");
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldAddSecurityRequirement()
    {
        // Assert
        _options.SwaggerGeneratorOptions.SecurityRequirements.Should().NotBeEmpty();
    }

    [Fact]
    public void SecurityRequirement_ShouldReferenceBearerScheme()
    {
        // Act
        var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();

        // Assert
        var bearerScheme = securityRequirement.Keys.FirstOrDefault(k => k.Reference?.Id == "Bearer");
        bearerScheme.Should().NotBeNull();
    }

    [Fact]
    public void SecurityRequirement_ShouldHaveCorrectReferenceType()
    {
        // Act
        var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();
        var bearerScheme = securityRequirement.Keys.First(k => k.Reference?.Id == "Bearer");

        // Assert
        bearerScheme.Reference.Type.Should().Be(ReferenceType.SecurityScheme);
    }

    [Fact]
    public void SecurityRequirement_ShouldHaveEmptyScopes()
    {
        // Act
        var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();
        var bearerScheme = securityRequirement.Keys.First(k => k.Reference?.Id == "Bearer");
        var scopes = securityRequirement[bearerScheme];

        // Assert
        scopes.Should().BeEmpty();
    }

    [Fact]
    public void BearerSecurityDefinition_ShouldHaveExampleInDescription()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Description.Should().Contain("Example:");
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldHaveOnlyOneBearerDefinition()
    {
        // Act
        var bearerDefinitions = _options.SwaggerGeneratorOptions.SecuritySchemes
            .Where(kvp => kvp.Key == "Bearer" || kvp.Value.Scheme == "bearer");

        // Assert
        bearerDefinitions.Should().HaveCount(1);
    }

    [Fact]
    public void AddSwaggerDocumentation_ShouldHaveOnlyOneSecurityRequirement()
    {
        // Assert
        _options.SwaggerGeneratorOptions.SecurityRequirements.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("Bearer")]
    [InlineData("bearer")]
    public void BearerSecurityDefinition_ShouldMatchScheme(string expectedScheme)
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Scheme.Should().BeEquivalentTo(expectedScheme);
    }

    [Fact]
    public void BearerSecurityDefinition_DescriptionShouldIncludeTokenFormat()
    {
        // Act
        var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        bearerScheme.Description.Should().Contain("eyJ"); // JWT token start pattern
    }
}

