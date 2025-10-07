using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
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
            Assert.True(response.Headers.ContainsKey("Correlation-Id"));
            var header = response.Headers["Correlation-Id"];
            Assert.Contains("correlation ID", header.Description);
            Assert.Equal("string", header.Schema.Type);
            Assert.Equal("uuid", header.Schema.Format);
            Assert.NotNull(header.Schema.Example);
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
        Assert.NotNull(_operation.Responses);
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
        Assert.NotNull(_operation.Responses["200"].Headers);
        Assert.True(_operation.Responses["200"].Headers.ContainsKey("Correlation-Id"));
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
        Assert.Same(existingHeader, _operation.Responses["200"].Headers["Correlation-Id"]);
        Assert.Equal("Custom description", _operation.Responses["200"].Headers["Correlation-Id"].Description);
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
        Assert.Equal("uuid", header.Schema.Format);
        Assert.Equal("string", header.Schema.Type);
    }

    private static OperationFilterContext CreateOperationFilterContext()
    {
        var apiDescription = new ApiDescription();
        var schemaRepository = new SchemaRepository();
        var methodInfo = typeof(CommonResponseHeadersFilterTests).GetMethod(nameof(CreateOperationFilterContext));
        
        return new OperationFilterContext(
            apiDescription,
            new TestSchemaGenerator(),
            schemaRepository,
            methodInfo!);
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
}

