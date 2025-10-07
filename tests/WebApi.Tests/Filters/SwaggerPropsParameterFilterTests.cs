using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebApi.Attributes;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests.Filters;

public class SwaggerPropsParameterFilterTests
{
    private readonly SwaggerPropsParameterFilter _sut;

    public SwaggerPropsParameterFilterTests()
    {
        _sut = new SwaggerPropsParameterFilter();
    }

    [Fact]
    public void Apply_ShouldNotThrow_WhenParameterInfoIsNull()
    {
        // Arrange
        var parameter = new OpenApiParameter
        {
            Name = "test-param",
            In = ParameterLocation.Query
        };

        var context = CreateParameterFilterContext(null);

        // Act
        var act = () => _sut.Apply(parameter, context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_ShouldApplySwaggerPropsAttributes_ToParameterWithAttribute()
    {
        // Arrange
        var parameter = new OpenApiParameter
        {
            Name = "Correlation-Id",
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema()
        };

        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "correlationId");
        
        var context = CreateParameterFilterContext(parameterInfo);

        // Act
        _sut.Apply(parameter, context);

        // Assert
        parameter.Schema.Should().NotBeNull();
        parameter.Schema.Format.Should().Be("uuid");
        parameter.Schema.Example.Should().NotBeNull();
        parameter.Schema.Example.Should().BeOfType<OpenApiString>();
        var exampleString = parameter.Schema.Example as OpenApiString;
        exampleString!.Value.Should().Be("12345678-1234-1234-1234-123456789abc");
    }

    [Fact]
    public void Apply_ShouldApplyDescription_WhenSwaggerPropsHasDescription()
    {
        // Arrange
        var parameter = new OpenApiParameter
        {
            Name = "test-param",
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema()
        };

        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "userContext");
        
        var context = CreateParameterFilterContext(parameterInfo);

        // Act
        _sut.Apply(parameter, context);

        // Assert
        parameter.Description.Should().Be("Test description for user context");
    }

    [Fact]
    public void Apply_ShouldInitializeSchema_WhenSchemaIsNull()
    {
        // Arrange
        var parameter = new OpenApiParameter
        {
            Name = "test-param",
            In = ParameterLocation.Header,
            Schema = null
        };

        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "correlationId");
        
        var context = CreateParameterFilterContext(parameterInfo);

        // Act
        _sut.Apply(parameter, context);

        // Assert
        parameter.Schema.Should().NotBeNull();
    }

    private static ParameterFilterContext CreateParameterFilterContext(ParameterInfo? parameterInfo)
    {
        var schemaRepository = new SchemaRepository();
        var apiParameterDescription = new ApiParameterDescription();

        return new ParameterFilterContext(
            apiParameterDescription,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository,
            null, // propertyInfo
            parameterInfo);
    }

    // Test class for reflection
    private class TestClass
    {
        public void TestMethod(
            [SwaggerProps(Format = "uuid", Example = "12345678-1234-1234-1234-123456789abc")]
            Guid correlationId,
            [SwaggerProps(Description = "Test description for user context")]
            string userContext)
        {
        }
    }
}

