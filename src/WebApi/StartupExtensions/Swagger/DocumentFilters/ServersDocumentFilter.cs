using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.DocumentFilters;

/// <summary>
/// Document filter to add server configurations to the OpenAPI document
/// </summary>
public class ServersDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = "https://localhost:60983", Description = "Local Development" },
            new OpenApiServer { Url = "https://dev.local", Description = "Development Environment" },
            new OpenApiServer { Url = "https://qa.local", Description = "QA Environment" },
            new OpenApiServer { Url = "https://uat.local", Description = "UAT Environment" }
        };
    }
}

