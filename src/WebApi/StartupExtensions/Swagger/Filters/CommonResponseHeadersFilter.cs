using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Operation filter to add common response headers to all operations
/// </summary>
public class CommonResponseHeadersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= new OpenApiResponses();
        
        // Add common response headers to all response codes
        foreach (var response in operation.Responses.Values)
        {
            response.Headers ??= new Dictionary<string, OpenApiHeader>();
            
            if (!response.Headers.ContainsKey("Correlation-Id"))
            {
                response.Headers["Correlation-Id"] = new OpenApiHeader
                {
                    Description = "The correlation ID for this request (same as request header or auto-generated if not provided)",
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "uuid",
                        Example = new Microsoft.OpenApi.Any.OpenApiString("12345678-1234-1234-1234-123456789abc")
                    }
                };
            }

            if (!response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                response.Headers["Access-Control-Allow-Origin"] = new OpenApiHeader
                {
                    Description = "Indicates which origin(s) are allowed to access the resource. '*' allows all origins.",
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new Microsoft.OpenApi.Any.OpenApiString("*")
                    }
                };
            }
        }
    }
}

