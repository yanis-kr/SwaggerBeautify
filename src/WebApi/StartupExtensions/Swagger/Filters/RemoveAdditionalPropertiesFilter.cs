using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Schema filter to remove additionalProperties from object schemas
/// </summary>
public class RemoveAdditionalPropertiesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type == "object")
        {
            schema.AdditionalPropertiesAllowed = true;
            schema.AdditionalProperties = null;
        }
    }
}

