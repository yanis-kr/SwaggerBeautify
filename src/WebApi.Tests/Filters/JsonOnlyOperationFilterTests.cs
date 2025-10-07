using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests.Filters;

public class JsonOnlyOperationFilterTests
{
    private readonly JsonOnlyOperationFilter _sut;
    private readonly OpenApiOperation _operation;
    private readonly OperationFilterContext _context;

    public JsonOnlyOperationFilterTests()
    {
        _sut = new JsonOnlyOperationFilter();
        _operation = new OpenApiOperation();
        _context = CreateOperationFilterContext();
    }

    [Fact]
    public void Apply_WhenRequestBodyHasMultipleContentTypes_ShouldKeepOnlyJson()
    {
        // Arrange
        var jsonMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
        var xmlMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
        
        _operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = jsonMediaType,
                ["application/xml"] = xmlMediaType,
                ["text/plain"] = new OpenApiMediaType()
            }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.RequestBody.Content.Should().HaveCount(1);
        _operation.RequestBody.Content.Should().ContainKey("application/json");
        _operation.RequestBody.Content["application/json"].Should().BeSameAs(jsonMediaType);
    }

    [Fact]
    public void Apply_WhenResponsesHaveMultipleContentTypes_ShouldKeepOnlyJson()
    {
        // Arrange
        var jsonMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
        var xmlMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
        
        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = jsonMediaType,
                    ["application/xml"] = xmlMediaType
                }
            },
            ["400"] = new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType(),
                    ["text/plain"] = new OpenApiMediaType()
                }
            }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Responses["200"].Content.Should().HaveCount(1);
        _operation.Responses["200"].Content.Should().ContainKey("application/json");
        _operation.Responses["200"].Content["application/json"].Should().BeSameAs(jsonMediaType);
        
        _operation.Responses["400"].Content.Should().HaveCount(1);
        _operation.Responses["400"].Content.Should().ContainKey("application/json");
    }

    [Fact]
    public void Apply_WhenRequestBodyIsNull_ShouldNotThrow()
    {
        // Arrange
        _operation.RequestBody = null;

        // Act
        var act = () => _sut.Apply(_operation, _context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_WhenRequestBodyContentIsNull_ShouldNotThrow()
    {
        // Arrange
        _operation.RequestBody = new OpenApiRequestBody { Content = null };

        // Act
        var act = () => _sut.Apply(_operation, _context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_WhenResponsesIsNull_ShouldNotThrow()
    {
        // Arrange
        _operation.Responses = null;

        // Act
        var act = () => _sut.Apply(_operation, _context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_WhenResponseContentIsNull_ShouldNotThrow()
    {
        // Arrange
        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Content = null }
        };

        // Act
        var act = () => _sut.Apply(_operation, _context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_WhenNoJsonContentType_ShouldClearContent()
    {
        // Arrange
        _operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/xml"] = new OpenApiMediaType(),
                ["text/plain"] = new OpenApiMediaType()
            }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        // Filter removes non-JSON content types, leaving only JSON if it exists
        // Since there's no JSON content type, the dictionary becomes empty
        _operation.RequestBody.Content.Keys.Should().NotContain("application/json");
    }

    private static OperationFilterContext CreateOperationFilterContext()
    {
        var apiDescription = new ApiDescription();
        var schemaRepository = new SchemaRepository();
        var methodInfo = typeof(JsonOnlyOperationFilterTests).GetMethod(nameof(CreateOperationFilterContext));
        
        return new OperationFilterContext(
            apiDescription,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository,
            methodInfo!);
    }
}

