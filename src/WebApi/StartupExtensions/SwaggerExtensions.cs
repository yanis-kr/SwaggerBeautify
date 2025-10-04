using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                    $"WebApi {description.GroupName.ToUpperInvariant()}");
            }
            
            c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
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
