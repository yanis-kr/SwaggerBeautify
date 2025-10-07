using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests.Filters;

public class CommonResponseHeadersFilterTests
{
    private readonly CommonResponseHeadersFilter _sut;
    private readonly OpenApiOperation _operation;
    private readonly OperationFilterContext _context;

    public CommonResponseHeadersFilterTests()
    {
        _sut = new CommonResponseHeadersFilter();
        _operation = new OpenApiOperation();
        _context = CreateOperationFilterContext();
    }

    [Fact]
    public void Apply_ShouldAddCorrelationIdToAllResponses()
    {
        // Arrange
        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Success" },
            ["400"] = new OpenApiResponse { Description = "Bad Request" },
            ["404"] = new OpenApiResponse { Description = "Not Found" }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        foreach (var response in _operation.Responses.Values)
        {
            response.Headers.Should().ContainKey("Correlation-Id");
            var header = response.Headers["Correlation-Id"];
            header.Description.Should().Contain("correlation ID");
            header.Schema.Type.Should().Be("string");
            header.Schema.Format.Should().Be("uuid");
            header.Schema.Example.Should().NotBeNull();
        }
    }

    [Fact]
    public void Apply_WhenResponsesIsNull_ShouldInitializeResponses()
    {
        // Arrange
        _operation.Responses = null;

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Responses.Should().NotBeNull();
    }

    [Fact]
    public void Apply_WhenResponseHeadersIsNull_ShouldInitializeHeaders()
    {
        // Arrange
        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse 
            { 
                Description = "Success",
                Headers = null
            }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Responses["200"].Headers.Should().NotBeNull();
        _operation.Responses["200"].Headers.Should().ContainKey("Correlation-Id");
    }

    [Fact]
    public void Apply_WhenCorrelationIdAlreadyExists_ShouldNotOverwrite()
    {
        // Arrange
        var existingHeader = new OpenApiHeader
        {
            Description = "Custom description",
            Schema = new OpenApiSchema { Type = "string" }
        };

        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse 
            { 
                Description = "Success",
                Headers = new Dictionary<string, OpenApiHeader>
                {
                    ["Correlation-Id"] = existingHeader
                }
            }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Responses["200"].Headers["Correlation-Id"].Should().BeSameAs(existingHeader);
        _operation.Responses["200"].Headers["Correlation-Id"].Description.Should().Be("Custom description");
    }

    [Fact]
    public void Apply_ShouldSetCorrectSchemaFormat()
    {
        // Arrange
        _operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Success" }
        };

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        var header = _operation.Responses["200"].Headers["Correlation-Id"];
        header.Schema.Format.Should().Be("uuid");
        header.Schema.Type.Should().Be("string");
    }

    private static OperationFilterContext CreateOperationFilterContext()
    {
        var apiDescription = new ApiDescription();
        var schemaRepository = new SchemaRepository();
        var methodInfo = typeof(CommonResponseHeadersFilterTests).GetMethod(nameof(CreateOperationFilterContext));
        
        return new OperationFilterContext(
            apiDescription,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository,
            methodInfo!);
    }
}

