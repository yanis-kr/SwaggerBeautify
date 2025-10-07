using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests.Filters;

public class CorrelationIdOperationFilterTests
{
    private readonly CorrelationIdOperationFilter _sut;
    private readonly OpenApiOperation _operation;
    private readonly OperationFilterContext _context;

    public CorrelationIdOperationFilterTests()
    {
        _sut = new CorrelationIdOperationFilter();
        _operation = new OpenApiOperation();
        _context = CreateOperationFilterContext();
    }

    [Fact]
    public void Apply_ShouldAddCorrelationIdParameter()
    {
        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Parameters.Should().NotBeNull();
        _operation.Parameters.Should().HaveCount(1);
        
        var parameter = _operation.Parameters.First();
        parameter.Name.Should().Be("Correlation-Id");
        parameter.In.Should().Be(ParameterLocation.Header);
        parameter.Required.Should().BeFalse();
        parameter.Description.Should().Contain("correlation ID");
        parameter.Schema.Type.Should().Be("string");
        parameter.Schema.Format.Should().Be("uuid");
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
        }
    }

    [Fact]
    public void Apply_WhenParametersIsNull_ShouldInitializeParametersList()
    {
        // Arrange
        _operation.Parameters = null;

        // Act
        _sut.Apply(_operation, _context);

        // Assert
        _operation.Parameters.Should().NotBeNull();
        _operation.Parameters.Should().HaveCount(1);
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
    public void Apply_ShouldAddExampleValue()
    {
        // Act
        _sut.Apply(_operation, _context);

        // Assert
        var parameter = _operation.Parameters.First();
        parameter.Schema.Example.Should().NotBeNull();
    }

    private static OperationFilterContext CreateOperationFilterContext()
    {
        var apiDescription = new ApiDescription();
        var schemaRepository = new SchemaRepository();
        var methodInfo = typeof(CorrelationIdOperationFilterTests).GetMethod(nameof(CreateOperationFilterContext));
        
        return new OperationFilterContext(
            apiDescription,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository,
            methodInfo!);
    }
}

