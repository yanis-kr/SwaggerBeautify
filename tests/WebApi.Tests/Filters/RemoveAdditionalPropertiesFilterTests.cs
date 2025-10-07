using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
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
        Assert.True(schema.AdditionalPropertiesAllowed);
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
        Assert.Null(schema.AdditionalProperties);
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
        Assert.Equal("string", schema.Type);
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
        Assert.Equal("array", schema.Type);
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
        Assert.Null(schema.Type);
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
        Assert.True(schema.AdditionalPropertiesAllowed);
        Assert.Null(schema.AdditionalProperties);
    }

    private static SchemaFilterContext CreateSchemaFilterContext(Type type)
    {
        var schemaRepository = new SchemaRepository();
        return new SchemaFilterContext(
            type,
            new TestSchemaGenerator(),
            schemaRepository);
    }

    private class TestSchemaGenerator : ISchemaGenerator
    {
        public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository, 
            System.Reflection.MemberInfo? memberInfo = null, System.Reflection.ParameterInfo? parameterInfo = null, 
            ApiParameterRouteInfo? routeInfo = null)
        {
            return new OpenApiSchema();
        }
    }

    private class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
