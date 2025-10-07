using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Attributes;

namespace WebApi.StartupExtensions.Swagger.Filters;

/// <summary>
/// Schema filter to process SwaggerProps attributes and apply them to schemas
/// </summary>
public class SwaggerPropsSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Check for SwaggerProps attribute on the type
        var swaggerPropsAttribute = context.Type.GetCustomAttributes(typeof(SwaggerPropsAttribute), false)
            .FirstOrDefault() as SwaggerPropsAttribute;

        if (swaggerPropsAttribute != null)
        {
            // Check if entire class should be hidden
            if (swaggerPropsAttribute.Hide)
            {
                // Mark schema as empty to effectively hide it
                schema.Properties?.Clear();
                schema.Type = null;
                schema.Description = "Hidden from API documentation";
                return;
            }
            else
            {
                ApplySwaggerProps(schema, swaggerPropsAttribute);
            }
        }

        // Check for SwaggerProps attributes on properties
        if (schema.Properties != null)
        {
            var propertiesToRemove = new List<string>();

            foreach (var property in context.Type.GetProperties())
            {
                var propSwaggerPropsAttribute = property.GetCustomAttributes(typeof(SwaggerPropsAttribute), false)
                    .FirstOrDefault() as SwaggerPropsAttribute;

                var camelCaseName = ToCamelCase(property.Name);

                if (propSwaggerPropsAttribute != null)
                {
                    // Check if property should be hidden
                    if (propSwaggerPropsAttribute.Hide)
                    {
                        propertiesToRemove.Add(camelCaseName);
                    }
                    else if (schema.Properties.ContainsKey(camelCaseName))
                    {
                        ApplySwaggerProps(schema.Properties[camelCaseName], propSwaggerPropsAttribute);
                    }
                }
            }

            // Remove hidden properties from schema
            foreach (var propertyName in propertiesToRemove)
            {
                schema.Properties.Remove(propertyName);
            }
        }

        // Legacy support for ExampleAttribute
        var exampleAttribute = context.Type.GetCustomAttributes(typeof(ExampleAttribute), false)
            .FirstOrDefault() as ExampleAttribute;

        if (exampleAttribute != null)
        {
            schema.Example = ConvertToOpenApiAny(exampleAttribute.Example);
        }

        // Legacy support for Example attributes on properties
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

    private static void ApplySwaggerProps(OpenApiSchema schema, SwaggerPropsAttribute swaggerProps)
    {
        if (swaggerProps.Example != null)
        {
            schema.Example = ConvertToOpenApiAny(swaggerProps.Example);
        }

        if (!string.IsNullOrEmpty(swaggerProps.Format))
        {
            schema.Format = swaggerProps.Format;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Description))
        {
            schema.Description = swaggerProps.Description;
        }

        if (swaggerProps.ReadOnly)
        {
            schema.ReadOnly = true;
        }

        if (swaggerProps.WriteOnly)
        {
            schema.WriteOnly = true;
        }

        if (swaggerProps.Minimum.HasValue)
        {
            schema.Minimum = (decimal)swaggerProps.Minimum.Value;
        }

        if (swaggerProps.Maximum.HasValue)
        {
            schema.Maximum = (decimal)swaggerProps.Maximum.Value;
        }

        if (swaggerProps.MinLength.HasValue)
        {
            schema.MinLength = swaggerProps.MinLength.Value;
        }

        if (swaggerProps.MaxLength.HasValue)
        {
            schema.MaxLength = swaggerProps.MaxLength.Value;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Pattern))
        {
            schema.Pattern = swaggerProps.Pattern;
        }

        if (swaggerProps.Nullable.HasValue)
        {
            schema.Nullable = swaggerProps.Nullable.Value;
        }

        if (!string.IsNullOrEmpty(swaggerProps.Deprecated))
        {
            schema.Deprecated = true;
            if (string.IsNullOrEmpty(schema.Description))
            {
                schema.Description = $"DEPRECATED: {swaggerProps.Deprecated}";
            }
            else
            {
                schema.Description += $" DEPRECATED: {swaggerProps.Deprecated}";
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

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}

