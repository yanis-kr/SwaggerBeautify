# Swagger Configuration - Pragmatic SOLID Design

This folder contains modular, single-responsibility static classes for Swagger/OpenAPI setup.

## Architecture: Static Utility Classes

**Why static classes instead of interfaces?**

- ✅ **Simpler** - No unnecessary abstraction layer
- ✅ **YAGNI** - We don't need runtime polymorphism or strategy swapping
- ✅ **Direct** - Clear, explicit method calls
- ✅ **Pragmatic** - Achieves SOLID goals without over-engineering
- ✅ **Easy to understand** - No interface indirection

## Structure

```
Configuration/
├── SwaggerDocumentConfiguration.cs             # Document metadata
├── SwaggerSecurityConfiguration.cs             # JWT Bearer security
├── SwaggerXmlCommentsConfiguration.cs          # XML documentation
├── SwaggerFiltersConfiguration.cs              # All filters registration
└── SwaggerTypeMappingsConfiguration.cs         # Type mappings
```

## SOLID Principles (Pragmatically Applied)

### ✅ Single Responsibility Principle (SRP)

Each configuration class has ONE reason to change:

- **SwaggerDocumentConfiguration** - Only changes when document metadata needs updating
- **SwaggerSecurityConfiguration** - Only changes when security requirements change
- **SwaggerXmlCommentsConfiguration** - Only changes when XML comment handling changes
- **SwaggerFiltersConfiguration** - Only changes when filters need to be added/removed
- **SwaggerTypeMappingsConfiguration** - Only changes when type mappings change

### ✅ Open/Closed Principle (OCP)

- Easy to add new configuration classes
- Existing configurations don't need modification
- Just add a new static class and call it

### ⚠️ Liskov Substitution Principle (LSP)

- Not applicable (using static classes, not polymorphism)
- **Conscious trade-off** for simplicity

### ✅ Interface Segregation Principle (ISP)

- Each class exposes only one `Configure` method
- No unnecessary methods or properties
- Minimal, focused API

### ⚠️ Dependency Inversion Principle (DIP)

- **Trade-off**: Direct dependencies on concrete static classes
- **Why it's okay**: These are configuration utilities, not business logic
- **Benefit**: Simpler, more maintainable code
- **Future-proof**: Easy to extract interface if polymorphism becomes necessary

## Usage

### Orchestrator (SwaggerServiceExtensions)

```csharp
public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        // Simple, explicit calls - no loops, no abstractions
        SwaggerDocumentConfiguration.Configure(options);
        SwaggerSecurityConfiguration.Configure(options);
        SwaggerXmlCommentsConfiguration.Configure(options);
        SwaggerFiltersConfiguration.Configure(options);
        SwaggerTypeMappingsConfiguration.Configure(options);
    });

    return services;
}
```

**Compare to over-engineered version:**

```csharp
// ❌ Unnecessary complexity
var configurations = new List<ISwaggerConfiguration> { ... };
foreach (var config in configurations) config.Configure(options);

// ✅ Simple and clear
SwaggerDocumentConfiguration.Configure(options);
SwaggerSecurityConfiguration.Configure(options);
```

## Configuration Classes

### 1. SwaggerDocumentConfiguration

**Responsibility**: Document metadata (title, version, contact, license)

```csharp
public static class SwaggerDocumentConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { ... });
        options.CustomSchemaIds(type => type.Name);
    }
}
```

### 2. SwaggerSecurityConfiguration

**Responsibility**: JWT Bearer authentication setup

```csharp
public static class SwaggerSecurityConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", ...);
        options.AddSecurityRequirement(...);
    }
}
```

### 3. SwaggerXmlCommentsConfiguration

**Responsibility**: XML documentation comments inclusion

```csharp
public static class SwaggerXmlCommentsConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
}
```

### 4. SwaggerFiltersConfiguration

**Responsibility**: Registering all Swagger filters

```csharp
public static class SwaggerFiltersConfiguration
{
    public static void Configure(SwaggerGenOptions options)
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
public static class SwaggerTypeMappingsConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.MapType<DateTime>(() => ...);
        options.MapType<Guid>(() => ...);
    }
}
```

## Benefits

### Maintainability

- ✅ Each configuration is isolated and easy to understand
- ✅ Changes to one aspect don't affect others
- ✅ Clear separation of concerns
- ✅ No unnecessary abstractions to maintain

### Testability

- ✅ Each configuration can be unit tested independently
- ✅ Direct method calls - easy to test
- ✅ No mocking needed for configuration classes

### Extensibility

- ✅ Add new configurations by creating a new static class
- ✅ No interface to implement
- ✅ Just add the method call to the orchestrator

### Readability

- ✅ Self-documenting class names
- ✅ Clear, explicit method calls
- ✅ No interface indirection
- ✅ Easy to locate specific configurations

## Adding New Configurations

1. Create a new static class with a `Configure` method
2. Add the method call in `SwaggerServiceExtensions`

### Example: Adding Custom Headers

```csharp
public static class SwaggerCustomHeadersConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<CustomHeaderOperationFilter>();
    }
}
```

Then in `SwaggerServiceExtensions`:

```csharp
SwaggerDocumentConfiguration.Configure(options);
SwaggerSecurityConfiguration.Configure(options);
SwaggerCustomHeadersConfiguration.Configure(options);  // ← Add here
...
```

## Philosophy: Pragmatic SOLID

### When to Use Interfaces

✅ **Use interfaces when:**

- Multiple implementations exist
- Runtime polymorphism is needed
- Dependency injection of different strategies
- Mocking for unit tests is required

### When NOT to Use Interfaces

✅ **Skip interfaces when:**

- Only one implementation exists
- Configuration is static
- No runtime behavior changes
- Adding complexity without benefit

### Our Choice

We chose **static utility classes** because:

1. **SOLID goals achieved** - Single responsibility, open/closed
2. **Simpler** - No unnecessary abstraction
3. **Pragmatic** - Right level of design for the problem
4. **Maintainable** - Easy to understand and modify
5. **Future-proof** - Can add interface later if needed

## Comparison: Over-Engineering vs Pragmatic

### Over-Engineered ❌

```csharp
// Interface
public interface ISwaggerConfiguration
{
    void Configure(SwaggerGenOptions options);
}

// Implementations
public class SwaggerDocumentConfiguration : ISwaggerConfiguration { ... }

// Usage
var configs = new List<ISwaggerConfiguration>
{
    new SwaggerDocumentConfiguration(),
    ...
};
foreach (var config in configs) config.Configure(options);
```

**Problems:**

- Unnecessary interface
- Object instantiation overhead
- List and loop for no reason
- More code to maintain

### Pragmatic ✅

```csharp
// Just static classes
public static class SwaggerDocumentConfiguration
{
    public static void Configure(SwaggerGenOptions options) { ... }
}

// Usage
SwaggerDocumentConfiguration.Configure(options);
SwaggerSecurityConfiguration.Configure(options);
```

**Benefits:**

- Simple and direct
- No unnecessary abstractions
- Clear intent
- Less code

## When to Refactor to Interfaces

Consider adding interfaces **ONLY IF**:

1. You need to swap implementations at runtime
2. You need to mock configurations in tests
3. You have multiple alternative configurations
4. You need dependency injection

**Until then**: Keep it simple! ✅
