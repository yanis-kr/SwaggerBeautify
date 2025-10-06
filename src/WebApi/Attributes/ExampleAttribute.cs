using System.Text.Json;

namespace WebApi.Attributes;

/// <summary>
/// Attribute to define example values for Swagger documentation
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
