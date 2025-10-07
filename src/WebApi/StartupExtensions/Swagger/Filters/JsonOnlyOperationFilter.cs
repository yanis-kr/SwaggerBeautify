using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Operation filter to restrict request and response content to application/json only
/// </summary>
public class JsonOnlyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Filter request body content to only include application/json
        if (operation.RequestBody?.Content != null)
        {
            var requestBodyContent = operation.RequestBody.Content;
            var jsonContent = requestBodyContent.FirstOrDefault(c => c.Key == "application/json");
            
            if (jsonContent.Value != null)
            {
                operation.RequestBody.Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", jsonContent.Value }
                };
            }
        }

        // Filter response content to only include application/json
        if (operation.Responses != null)
        {
            foreach (var response in operation.Responses.Values)
            {
                if (response.Content != null)
                {
                    var responseContent = response.Content;
                    var jsonContent = responseContent.FirstOrDefault(c => c.Key == "application/json");
                    
                    if (jsonContent.Value != null)
                    {
                        response.Content = new Dictionary<string, OpenApiMediaType>
                        {
                            { "application/json", jsonContent.Value }
                        };
                    }
                }
            }
        }
    }
}

