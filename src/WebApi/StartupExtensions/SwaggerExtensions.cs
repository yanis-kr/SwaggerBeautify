using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Attributes;

namespace WebApi.StartupExtensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "WebApi",
                    Version = description.ApiVersion.ToString(),
                    Description = "A sample API with Authors and Books"
                });
            }
            

            // Add operation filter to include Correlation-Id header in all operations
            c.OperationFilter<CorrelationIdOperationFilter>();

            // Add operation filter to set example values for Author ID parameters
            c.OperationFilter<AuthorIdExampleFilter>();

            // Add schema filter to process Example attributes
            c.SchemaFilter<ExampleSchemaFilter>();

            // Add document filter to include servers configuration
            c.DocumentFilter<ServersDocumentFilter>();

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions.Reverse())
            {
                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml",
                    $"WebApi {description.GroupName.ToUpperInvariant()}");
            }

            //options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "My Custom API V1 (YAML)");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}

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

public class ServersDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer
            {
                Url = "https://dev.local",
                Description = "Development Environment"
            },
            new OpenApiServer
            {
                Url = "https://qa.local",
                Description = "QA Environment"
            }
        };
    }
}

public class AuthorIdExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if this is an Authors controller endpoint
        var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];
        if (controllerName?.ToLower() != "authors")
            return;

        // Find the 'id' parameter and add example value
        var idParameter = operation.Parameters?.FirstOrDefault(p => 
            p.Name.Equals("id", StringComparison.OrdinalIgnoreCase) && 
            p.In == ParameterLocation.Path);

        if (idParameter != null)
        {
            idParameter.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
        }
    }
}

public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Check for Example attribute on the type
        var exampleAttribute = context.Type.GetCustomAttributes(typeof(ExampleAttribute), false)
            .FirstOrDefault() as ExampleAttribute;

        if (exampleAttribute != null)
        {
            schema.Example = ConvertToOpenApiAny(exampleAttribute.Example);
        }

        // Check for Example attributes on properties
        if (schema.Properties != null)
        {
            foreach (var property in context.Type.GetProperties())
            {
                var propExampleAttribute = property.GetCustomAttributes(typeof(ExampleAttribute), false)
                    .FirstOrDefault() as ExampleAttribute;

                if (propExampleAttribute != null && schema.Properties.ContainsKey(ToCamelCase(property.Name)))
                {
                    schema.Properties[ToCamelCase(property.Name)].Example = ConvertToOpenApiAny(propExampleAttribute.Example);
                }
            }
        }
    }

    private static Microsoft.OpenApi.Any.IOpenApiAny ConvertToOpenApiAny(object value)
    {
        return value switch
        {
            string str => new Microsoft.OpenApi.Any.OpenApiString(str),
            int integer => new Microsoft.OpenApi.Any.OpenApiInteger(integer),
            long longValue => new Microsoft.OpenApi.Any.OpenApiLong(longValue),
            double doubleValue => new Microsoft.OpenApi.Any.OpenApiDouble(doubleValue),
            float floatValue => new Microsoft.OpenApi.Any.OpenApiFloat(floatValue),
            bool boolValue => new Microsoft.OpenApi.Any.OpenApiBoolean(boolValue),
            DateTime dateTime => new Microsoft.OpenApi.Any.OpenApiDateTime(dateTime),
            _ => new Microsoft.OpenApi.Any.OpenApiString(value.ToString() ?? "")
        };
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}

