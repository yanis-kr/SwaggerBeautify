using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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

        // Act & Assert
        var exception = Record.Exception(() => _sut.Apply(parameter, context));
        Assert.Null(exception);
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
        Assert.NotNull(parameter.Schema);
        Assert.Equal("uuid", parameter.Schema.Format);
        Assert.NotNull(parameter.Schema.Example);
        Assert.IsType<OpenApiString>(parameter.Schema.Example);
        var exampleString = parameter.Schema.Example as OpenApiString;
        Assert.Equal("12345678-1234-1234-1234-123456789abc", exampleString!.Value);
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
        Assert.Equal("Test description for user context", parameter.Description);
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
        Assert.NotNull(parameter.Schema);
    }

    private static ParameterFilterContext CreateParameterFilterContext(ParameterInfo? parameterInfo)
    {
        var schemaRepository = new SchemaRepository();
        var apiParameterDescription = new ApiParameterDescription();

        return new ParameterFilterContext(
            apiParameterDescription,
            new TestSchemaGenerator(),
            schemaRepository,
            null, // propertyInfo
            parameterInfo);
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

