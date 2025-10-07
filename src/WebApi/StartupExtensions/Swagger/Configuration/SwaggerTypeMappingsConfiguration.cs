using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Configuration;

/// <summary>
/// Configures type mappings for consistent JSON schema generation
/// </summary>
public class SwaggerTypeMappingsConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.MapType<DateTime>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date-time",
            Example = new Microsoft.OpenApi.Any.OpenApiDateTime(DateTime.UtcNow)
        });

        options.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date",
            Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15")
        });

        options.MapType<TimeOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "time",
            Example = new Microsoft.OpenApi.Any.OpenApiString("14:30:00")
        });

        options.MapType<Guid>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "uuid",
            Example = new Microsoft.OpenApi.Any.OpenApiString("12345678-1234-1234-1234-123456789abc")
        });
    }
}

