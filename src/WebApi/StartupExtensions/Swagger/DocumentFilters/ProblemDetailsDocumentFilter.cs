using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.DocumentFilters;

/// <summary>
/// Document filter to explicitly add ProblemDetails schema to OpenAPI specification
/// </summary>
public class ProblemDetailsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Components ??= new OpenApiComponents();
        swaggerDoc.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

        // Add ProblemDetails schema if not already present
        if (!swaggerDoc.Components.Schemas.ContainsKey("ProblemDetails"))
        {
            swaggerDoc.Components.Schemas["ProblemDetails"] = new OpenApiSchema
            {
                Type = "object",
                Description = "A machine-readable format for specifying errors in HTTP API responses based on RFC 7807",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "A URI reference that identifies the problem type",
                        Example = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
                        Nullable = true
                    },
                    ["title"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "A short, human-readable summary of the problem type",
                        Example = new OpenApiString("One or more validation errors occurred."),
                        Nullable = true
                    },
                    ["status"] = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32",
                        Description = "The HTTP status code",
                        Example = new OpenApiInteger(400),
                        Nullable = true
                    },
                    ["detail"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "A human-readable explanation specific to this occurrence of the problem",
                        Example = new OpenApiString("The request contained invalid parameters."),
                        Nullable = true
                    },
                    ["instance"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "A URI reference that identifies the specific occurrence of the problem",
                        Example = new OpenApiString("/api/v1/books"),
                        Nullable = true
                    },
                    ["errors"] = new OpenApiSchema
                    {
                        Type = "object",
                        Description = "Validation errors grouped by field name",
                        AdditionalProperties = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        },
                        Example = new OpenApiObject
                        {
                            ["Title"] = new OpenApiArray
                            {
                                new OpenApiString("The Title field is required.")
                            },
                            ["AuthorId"] = new OpenApiArray
                            {
                                new OpenApiString("The AuthorId must be greater than 0.")
                            }
                        },
                        Nullable = true
                    },
                    ["traceId"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "The trace identifier for the request",
                        Example = new OpenApiString("00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"),
                        Nullable = true
                    }
                },
                AdditionalPropertiesAllowed = true,
                Example = new OpenApiObject
                {
                    ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
                    ["title"] = new OpenApiString("One or more validation errors occurred."),
                    ["status"] = new OpenApiInteger(400),
                    ["errors"] = new OpenApiObject
                    {
                        ["Title"] = new OpenApiArray { new OpenApiString("The Title field is required.") },
                        ["AuthorId"] = new OpenApiArray { new OpenApiString("The AuthorId must be greater than 0.") }
                    },
                    ["traceId"] = new OpenApiString("00-1234567890abcdef1234567890abcdef-1234567890abcdef-00")
                }
            };
        }
    }
}
