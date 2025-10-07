using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebApi.Attributes;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Parameter filter to process SwaggerProps attributes on action parameters
/// </summary>
public class SwaggerPropsParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        // Get the parameter info from the action parameter
        var parameterInfo = context.ParameterInfo;
        if (parameterInfo == null)
            return;

        // Check if this is a complex object with properties (like CommonParameters)
        var parameterType = parameterInfo.ParameterType;
        if (!parameterType.IsPrimitive && parameterType != typeof(string) && !parameterType.IsEnum)
        {
            // For complex types, look for properties with [FromHeader] attribute
            var properties = parameterType.GetProperties();
            foreach (var property in properties)
            {
                // Check for FromHeader attribute
                var fromHeaderAttr = property.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromHeaderAttribute>();
                if (fromHeaderAttr != null)
                {
                    // Get SwaggerProps attribute if present
                    var swaggerPropsAttribute = property.GetCustomAttribute<SwaggerPropsAttribute>();
                    
                    // Create parameter for this header property
                    var headerName = fromHeaderAttr.Name ?? property.Name;
                    
                    // Check if parameter already exists in the operation
                    // If not, this filter won't add it (that's handled by Swashbuckle's binding source inference)
                    // But we can enhance it if it exists
                }
            }
        }

        // Check for SwaggerProps attribute on the parameter itself
        var swaggerProps = parameterInfo.GetCustomAttribute<SwaggerPropsAttribute>();
        if (swaggerProps != null)
        {
            ApplySwaggerProps(parameter, swaggerProps);
        }

        // Also check SwaggerProps on properties of the parameter type if it's bound from a source
        if (parameter.In == ParameterLocation.Header)
        {
            // Find matching property in parameter type
            var property = parameterInfo.ParameterType.GetProperties()
                .FirstOrDefault(p => 
                {
                    var fromHeaderAttr = p.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromHeaderAttribute>();
                    return fromHeaderAttr != null && 
                           (fromHeaderAttr.Name ?? p.Name).Equals(parameter.Name, StringComparison.OrdinalIgnoreCase);
                });

            if (property != null)
            {
                var propSwaggerProps = property.GetCustomAttribute<SwaggerPropsAttribute>();
                if (propSwaggerProps != null)
                {
                    ApplySwaggerProps(parameter, propSwaggerProps);
                }
            }
        }
    }

    private static void ApplySwaggerProps(OpenApiParameter parameter, SwaggerPropsAttribute swaggerProps)
    {
        if (!string.IsNullOrEmpty(swaggerProps.Description))
        {
            parameter.Description = swaggerProps.Description;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Format))
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.Format = swaggerProps.Format;
        }

        if (swaggerProps.Example != null)
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.Example = ConvertToOpenApiAny(swaggerProps.Example);
        }

        if (swaggerProps.MinLength.HasValue)
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.MinLength = swaggerProps.MinLength.Value;
        }

        if (swaggerProps.MaxLength.HasValue)
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.MaxLength = swaggerProps.MaxLength.Value;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Pattern))
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.Pattern = swaggerProps.Pattern;
        }

        if (swaggerProps.Nullable.HasValue)
        {
            parameter.Schema ??= new OpenApiSchema();
            parameter.Schema.Nullable = swaggerProps.Nullable.Value;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Deprecated))
        {
            parameter.Deprecated = true;
            if (string.IsNullOrEmpty(parameter.Description))
            {
                parameter.Description = $"DEPRECATED: {swaggerProps.Deprecated}";
            }
            else
            {
                parameter.Description += $" DEPRECATED: {swaggerProps.Deprecated}";
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
            Guid guid => new Microsoft.OpenApi.Any.OpenApiString(guid.ToString()),
            _ => new Microsoft.OpenApi.Any.OpenApiString(value.ToString() ?? "")
        };
    }
}

