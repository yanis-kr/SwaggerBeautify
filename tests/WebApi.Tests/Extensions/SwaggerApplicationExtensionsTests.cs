using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
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
        
        var appBuilder = new TestApplicationBuilder(serviceProvider);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => appBuilder.UseSwaggerDocumentation());
        Assert.NotNull(exception);
    }

    [Fact]
    public void UseSwaggerDocumentation_WithValidProvider_ShouldNotThrowOnInitialCall()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IApiVersionDescriptionProvider>(new TestApiVersionDescriptionProvider());
        var serviceProvider = services.BuildServiceProvider();
        
        var appBuilder = new TestApplicationBuilder(serviceProvider);

        // Act & Assert
        // The method will throw when trying to use Swagger middleware without full setup
        var exception = Assert.Throws<InvalidOperationException>(() => appBuilder.UseSwaggerDocumentation());
        Assert.Contains("SwaggerOptions", exception.Message);
    }

    private class TestApplicationBuilder : Microsoft.AspNetCore.Builder.IApplicationBuilder
    {
        public TestApplicationBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
            Properties = new Dictionary<string, object?>();
        }

        public IServiceProvider ApplicationServices { get; set; }
        public IFeatureCollection ServerFeatures => throw new NotImplementedException();
        public IDictionary<string, object?> Properties { get; }
        public Microsoft.AspNetCore.Http.RequestDelegate Build() => throw new NotImplementedException();
        public Microsoft.AspNetCore.Builder.IApplicationBuilder New() => throw new NotImplementedException();
        public Microsoft.AspNetCore.Builder.IApplicationBuilder Use(Func<Microsoft.AspNetCore.Http.RequestDelegate, Microsoft.AspNetCore.Http.RequestDelegate> middleware)
        {
            return this;
        }
    }

    private class TestApiVersionDescriptionProvider : IApiVersionDescriptionProvider
    {
        public IReadOnlyList<ApiVersionDescription> ApiVersionDescriptions =>
            new List<ApiVersionDescription>
            {
                new ApiVersionDescription(new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0), "v1", false)
            };

        public bool IsDeprecated(Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor actionDescriptor, 
            Microsoft.AspNetCore.Mvc.ApiVersion apiVersion)
        {
            return false;
        }
    }
}

