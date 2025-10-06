using System.Text.Json;

namespace WebApi.Attributes;

/// <summary>
/// Comprehensive attribute for Swagger/OpenAPI documentation properties
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
public class SwaggerPropsAttribute : Attribute
{
    /// <summary>
    /// Example value for the property/class
    /// </summary>
    public object? Example { get; set; }

    /// <summary>
    /// Format specification (e.g., "date", "date-time", "email", "uri", "uuid", "password", "byte", "binary")
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Description for the property/class (overrides XML documentation)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the property is read-only
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Whether the property is write-only
    /// </summary>
    public bool WriteOnly { get; set; }

    /// <summary>
    /// Minimum value for numeric types
    /// </summary>
    public double? Minimum { get; set; }

    /// <summary>
    /// Maximum value for numeric types
    /// </summary>
    public double? Maximum { get; set; }

    /// <summary>
    /// Minimum length for string types
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Maximum length for string types
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Pattern for string validation (regex)
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Whether the property is nullable
    /// </summary>
    public bool? Nullable { get; set; }

    /// <summary>
    /// Deprecation notice
    /// </summary>
    public string? Deprecated { get; set; }

    /// <summary>
    /// Constructor for basic example value
    /// </summary>
    public SwaggerPropsAttribute(object example)
    {
        Example = example;
    }

    /// <summary>
    /// Constructor for format specification
    /// </summary>
    public SwaggerPropsAttribute(string format, object example)
    {
        Format = format;
        Example = example;
    }

    /// <summary>
    /// Default constructor for property-based configuration
    /// </summary>
    public SwaggerPropsAttribute()
    {
    }
}

/// <summary>
/// Legacy Example attribute for backward compatibility
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ExampleAttribute : Attribute
{
    public object Example { get; }

    public ExampleAttribute(object example)
    {
        Example = example;
    }
}

/// <summary>
/// Helper class to generate JSON examples from objects
/// </summary>
public static class ExampleHelper
{
    public static string ToJsonExample(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
