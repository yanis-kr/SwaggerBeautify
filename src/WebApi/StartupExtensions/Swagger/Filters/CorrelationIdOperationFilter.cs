using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Operation filter to add Correlation-Id header to all operations
/// </summary>
public class CorrelationIdOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Correlation-Id",
            In = ParameterLocation.Header,
            Description = "Optional correlation ID for request tracking. If not provided, a new GUID will be automatically generated and returned in the response headers.",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid",
                Example = new Microsoft.OpenApi.Any.OpenApiString("12345678-1234-1234-1234-123456789abc")
            }
        });

        // Add response header documentation
        operation.Responses ??= new OpenApiResponses();
        
        // Add Correlation-Id to all response codes
        foreach (var response in operation.Responses.Values)
        {
            response.Headers ??= new Dictionary<string, OpenApiHeader>();
            response.Headers["Correlation-Id"] = new OpenApiHeader
            {
                Description = "The correlation ID for this request (same as request header or auto-generated)",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid"
                }
            };
        }
    }
}

