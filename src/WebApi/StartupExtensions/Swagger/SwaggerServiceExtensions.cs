using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.DocumentFilters;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.StartupExtensions.Swagger;

/// <summary>
/// Extension methods for configuring Swagger services (builder.Services)
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Adds Swagger services and configuration to the service collection
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            ConfigureSwaggerGen(c);
            AddXmlComments(c);
            AddOperationFilters(c);
            AddSchemaFilters(c);
            AddDocumentFilters(c);
            AddTypeMappings(c);
        });

        return services;
    }

    #region Private Configuration Methods

    private static void ConfigureSwaggerGen(SwaggerGenOptions c)
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WebApi",
            Version = "v1",
            Description = "A sample Web API demonstrating Swagger documentation",
            Contact = new OpenApiContact
            {
                Name = "Your Name",
                Email = "your.email@example.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // Use fully qualified names for schema IDs
        c.CustomSchemaIds(type => type.FullName);
    }

    private static void AddXmlComments(SwaggerGenOptions c)
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }

    private static void AddOperationFilters(SwaggerGenOptions c)
    {
        c.OperationFilter<CorrelationIdOperationFilter>();
        c.OperationFilter<JsonOnlyOperationFilter>();
    }

    private static void AddSchemaFilters(SwaggerGenOptions c)
    {
        c.SchemaFilter<SwaggerPropsSchemaFilter>();
        c.SchemaFilter<RemoveAdditionalPropertiesFilter>();
    }

    private static void AddDocumentFilters(SwaggerGenOptions c)
    {
        c.DocumentFilter<ServersDocumentFilter>();
    }

    private static void AddTypeMappings(SwaggerGenOptions c)
    {
        // Ensure consistent JSON schema generation for common types
        c.MapType<DateTime>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date-time",
            Example = new Microsoft.OpenApi.Any.OpenApiDateTime(DateTime.UtcNow)
        });

        c.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date",
            Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15")
        });

        c.MapType<TimeOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "time",
            Example = new Microsoft.OpenApi.Any.OpenApiString("14:30:00")
        });

        c.MapType<Guid>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "uuid",
            Example = new Microsoft.OpenApi.Any.OpenApiString("12345678-1234-1234-1234-123456789abc")
        });
    }

    #endregion
}

