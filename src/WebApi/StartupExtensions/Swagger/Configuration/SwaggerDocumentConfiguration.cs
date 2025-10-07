using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Configuration;

/// <summary>
/// Configures Swagger document metadata
/// </summary>
public class SwaggerDocumentConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WebApi",
            Version = "1.2.3",
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

        // Use simple type names for schema IDs
        options.CustomSchemaIds(type => type.Name);
    }
}

