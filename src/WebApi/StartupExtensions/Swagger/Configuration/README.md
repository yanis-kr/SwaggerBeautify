# Swagger Configuration - SOLID Architecture

This folder contains modular, single-responsibility configuration classes for Swagger/OpenAPI setup.

## Architecture Pattern

Uses the **Strategy Pattern** with a common interface (`ISwaggerConfiguration`) to allow each configuration concern to be independently managed and tested.

## Structure

```
Configuration/
├── ISwaggerConfiguration.cs                    # Common interface
├── SwaggerDocumentConfiguration.cs             # Document metadata
├── SwaggerSecurityConfiguration.cs             # JWT Bearer security
├── SwaggerXmlCommentsConfiguration.cs          # XML documentation
├── SwaggerFiltersConfiguration.cs              # All filters registration
└── SwaggerTypeMappingsConfiguration.cs         # Type mappings
```

## SOLID Principles Applied

### Single Responsibility Principle (SRP) ✅

Each configuration class has ONE reason to change:

- `SwaggerDocumentConfiguration` - Only changes when document metadata needs updating
- `SwaggerSecurityConfiguration` - Only changes when security requirements change
- `SwaggerXmlCommentsConfiguration` - Only changes when XML comment handling changes
- `SwaggerFiltersConfiguration` - Only changes when filters need to be added/removed
- `SwaggerTypeMappingsConfiguration` - Only changes when type mappings change

### Open/Closed Principle (OCP) ✅

- Easy to add new configurations by implementing `ISwaggerConfiguration`
- Existing configurations don't need modification when adding new ones
- Main extension class is closed for modification, open for extension

### Liskov Substitution Principle (LSP) ✅

- All implementations properly implement `ISwaggerConfiguration`
- Can be substituted without affecting behavior
- Interface contract is maintained

### Interface Segregation Principle (ISP) ✅

- `ISwaggerConfiguration` has only one method: `Configure`
- No client is forced to depend on methods it doesn't use
- Focused, minimal interface

### Dependency Inversion Principle (DIP) ✅

- `SwaggerServiceExtensions` depends on `ISwaggerConfiguration` abstraction
- Concrete configurations can be changed without affecting the extension
- High-level module doesn't depend on low-level modules

## Usage

### Main Extension (Orchestrator)

```csharp
public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        var configurations = new List<ISwaggerConfiguration>
        {
            new SwaggerDocumentConfiguration(),
            new SwaggerSecurityConfiguration(),
            new SwaggerXmlCommentsConfiguration(),
            new SwaggerFiltersConfiguration(),
            new SwaggerTypeMappingsConfiguration()
        };

        foreach (var configuration in configurations)
        {
            configuration.Configure(options);
        }
    });

    return services;
}
```

## Configuration Classes

### 1. SwaggerDocumentConfiguration

**Responsibility**: Document metadata (title, version, contact, license)

```csharp
public class SwaggerDocumentConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { ... });
        options.CustomSchemaIds(type => type.Name);
    }
}
```

### 2. SwaggerSecurityConfiguration

**Responsibility**: JWT Bearer authentication setup

```csharp
public class SwaggerSecurityConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", ...);
        options.AddSecurityRequirement(...);
    }
}
```

### 3. SwaggerXmlCommentsConfiguration

**Responsibility**: XML documentation comments inclusion

```csharp
public class SwaggerXmlCommentsConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
}
```

### 4. SwaggerFiltersConfiguration

**Responsibility**: Registering all Swagger filters

```csharp
public class SwaggerFiltersConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<CorrelationIdOperationFilter>();
        options.SchemaFilter<SwaggerPropsSchemaFilter>();
        options.DocumentFilter<ServersDocumentFilter>();
    }
}
```

### 5. SwaggerTypeMappingsConfiguration

**Responsibility**: Custom type mappings for JSON schema

```csharp
public class SwaggerTypeMappingsConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.MapType<DateTime>(() => ...);
        options.MapType<Guid>(() => ...);
    }
}
```

## Benefits

### Maintainability

- Each configuration is isolated and easy to understand
- Changes to one aspect don't affect others
- Clear separation of concerns

### Testability

- Each configuration can be unit tested independently
- Mock `SwaggerGenOptions` for testing
- Easy to verify specific configurations

### Extensibility

- Add new configurations by implementing `ISwaggerConfiguration`
- No need to modify existing code
- Plugin-like architecture

### Readability

- Self-documenting class names
- Clear responsibility for each class
- Easy to locate specific configurations

## Adding New Configurations

1. Create a new class implementing `ISwaggerConfiguration`
2. Implement the `Configure` method
3. Add instance to the configurations list in `SwaggerServiceExtensions`

### Example: Adding API Key Security

```csharp
public class SwaggerApiKeyConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-API-KEY",
            Description = "API Key Authentication"
        });
    }
}
```

Then add to the list:

```csharp
var configurations = new List<ISwaggerConfiguration>
{
    new SwaggerDocumentConfiguration(),
    new SwaggerSecurityConfiguration(),
    new SwaggerApiKeyConfiguration(),  // ← New configuration
    ...
};
```

## Comparison: Before vs After

### Before (Monolithic)

```csharp
private static void ConfigureSwaggerGen(SwaggerGenOptions c) { ... }
private static void AddSecurity(SwaggerGenOptions c) { ... }
private static void AddXmlComments(SwaggerGenOptions c) { ... }
private static void AddOperationFilters(SwaggerGenOptions c) { ... }
private static void AddSchemaFilters(SwaggerGenOptions c) { ... }
private static void AddDocumentFilters(SwaggerGenOptions c) { ... }
private static void AddTypeMappings(SwaggerGenOptions c) { ... }
```

❌ Multiple responsibilities in one class  
❌ Tight coupling  
❌ Hard to test individually  
❌ Difficult to extend

### After (SOLID)

```csharp
ISwaggerConfiguration interface
  ├── SwaggerDocumentConfiguration
  ├── SwaggerSecurityConfiguration
  ├── SwaggerXmlCommentsConfiguration
  ├── SwaggerFiltersConfiguration
  └── SwaggerTypeMappingsConfiguration
```

✅ Single responsibility per class  
✅ Loose coupling through interface  
✅ Easy to test  
✅ Easy to extend

## Testing Strategy

Each configuration class can have dedicated tests:

```csharp
[Fact]
public void SwaggerDocumentConfiguration_ShouldSetCorrectVersion()
{
    var options = new SwaggerGenOptions();
    var config = new SwaggerDocumentConfiguration();

    config.Configure(options);

    options.SwaggerGeneratorOptions.SwaggerDocs["v1"]
        .Version.Should().Be("1.2.3");
}
```

## Best Practices

1. **Keep configurations focused** - One concern per class
2. **Use descriptive names** - Class name should reflect responsibility
3. **Document why, not what** - Explain reasoning in comments
4. **Test independently** - Each configuration should have its own tests
5. **Order matters** - Some configurations may depend on others running first
