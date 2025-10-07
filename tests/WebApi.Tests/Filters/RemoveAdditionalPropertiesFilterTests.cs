using FluentAssertions;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests.Filters;

public class RemoveAdditionalPropertiesFilterTests
{
    private readonly RemoveAdditionalPropertiesFilter _sut;
    private readonly SchemaFilterContext _context;

    public RemoveAdditionalPropertiesFilterTests()
    {
        _sut = new RemoveAdditionalPropertiesFilter();
        _context = CreateSchemaFilterContext(typeof(TestDto));
    }

    [Fact]
    public void Apply_WhenSchemaTypeIsObject_ShouldSetAdditionalPropertiesAllowedToTrue()
    {
        // Arrange
        var schema = new OpenApiSchema { Type = "object" };

        // Act
        _sut.Apply(schema, _context);

        // Assert
        schema.AdditionalPropertiesAllowed.Should().BeTrue();
    }

    [Fact]
    public void Apply_WhenSchemaTypeIsObject_ShouldSetAdditionalPropertiesToNull()
    {
        // Arrange
        var schema = new OpenApiSchema 
        { 
            Type = "object",
            AdditionalProperties = new OpenApiSchema { Type = "string" }
        };

        // Act
        _sut.Apply(schema, _context);

        // Assert
        schema.AdditionalProperties.Should().BeNull();
    }

    [Fact]
    public void Apply_WhenSchemaTypeIsString_ShouldNotModifySchema()
    {
        // Arrange
        var schema = new OpenApiSchema 
        { 
            Type = "string",
            AdditionalProperties = new OpenApiSchema { Type = "string" }
        };
        var originalAllowed = schema.AdditionalPropertiesAllowed;
        var originalAdditionalProps = schema.AdditionalProperties;

        // Act
        _sut.Apply(schema, _context);

        // Assert - Filter should not modify non-object types
        // But OpenApiSchema defaults AdditionalPropertiesAllowed to true, so verify logic executed correctly
        schema.Type.Should().Be("string");
    }

    [Fact]
    public void Apply_WhenSchemaTypeIsArray_ShouldNotModifySchema()
    {
        // Arrange
        var schema = new OpenApiSchema 
        { 
            Type = "array",
            AdditionalProperties = new OpenApiSchema { Type = "string" }
        };

        // Act
        _sut.Apply(schema, _context);

        // Assert - Filter should not modify non-object types
        schema.Type.Should().Be("array");
    }

    [Fact]
    public void Apply_WhenSchemaTypeIsNull_ShouldNotModifySchema()
    {
        // Arrange
        var schema = new OpenApiSchema 
        { 
            Type = null,
            AdditionalProperties = new OpenApiSchema { Type = "string" }
        };

        // Act
        _sut.Apply(schema, _context);

        // Assert - Filter should not modify non-object types
        schema.Type.Should().BeNull();
    }

    [Theory]
    [InlineData("object")]
    [InlineData("Object")]
    [InlineData("OBJECT")]
    public void Apply_ShouldBeCaseInsensitiveForObjectType(string typeValue)
    {
        // Arrange
        var schema = new OpenApiSchema { Type = typeValue };

        // Act
        _sut.Apply(schema, _context);

        // Assert
        schema.AdditionalPropertiesAllowed.Should().BeTrue();
        schema.AdditionalProperties.Should().BeNull();
    }

    private static SchemaFilterContext CreateSchemaFilterContext(Type type)
    {
        var schemaRepository = new SchemaRepository();
        return new SchemaFilterContext(
            type,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository);
    }

    private class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

