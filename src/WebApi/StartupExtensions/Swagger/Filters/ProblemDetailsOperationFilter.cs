using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Operation filter to automatically add ProblemDetails schema to all 4xx and 5xx error responses
/// </summary>
public class ProblemDetailsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= new OpenApiResponses();

        // Find all 4xx and 5xx responses that don't have content
        var errorResponses = operation.Responses
            .Where(r => r.Key.StartsWith("4") || r.Key.StartsWith("5"))
            .ToList();

        foreach (var errorResponse in errorResponses)
        {
            // If response has no content or empty content, add ProblemDetails
            if (errorResponse.Value.Content == null || !errorResponse.Value.Content.Any())
            {
                errorResponse.Value.Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                };
            }
        }
    }
}
