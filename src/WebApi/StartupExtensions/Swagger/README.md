# Swagger Configuration - SOLID Architecture

This folder contains all Swagger/OpenAPI related configuration following SOLID principles.

## Structure

```
Swagger/
├── SwaggerServiceExtensions.cs       # builder.Services.AddSwaggerDocumentation()
├── SwaggerApplicationExtensions.cs   # app.UseSwaggerDocumentation()
├── Filters/                          # IOperationFilter and ISchemaFilter implementations
│   ├── CorrelationIdOperationFilter.cs
│   ├── JsonOnlyOperationFilter.cs
│   ├── SwaggerPropsSchemaFilter.cs
│   └── RemoveAdditionalPropertiesFilter.cs
└── DocumentFilters/                  # IDocumentFilter implementations
    └── ServersDocumentFilter.cs
```

## Usage

### In Program.cs

```csharp
using WebApi.StartupExtensions.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure Swagger UI
app.UseSwaggerDocumentation();

app.Run();
```

## Extension Classes

### SwaggerServiceExtensions

**Purpose:** Configures Swagger generation services (used with `builder.Services`)

**Responsibilities:**

- Adds SwaggerGen services
- Configures OpenAPI document metadata
- Registers operation, schema, and document filters
- Sets up type mappings for common types
- Includes XML documentation comments

### SwaggerApplicationExtensions

**Purpose:** Configures Swagger UI middleware (used with `app`)

**Responsibilities:**

- Configures Swagger middleware
- Sets up SwaggerUI with custom settings
- Configures API versioning endpoints
- Sets UI preferences and features

## Filters

### Operation Filters (IOperationFilter)

Modify individual API operations (endpoints)

- **CorrelationIdOperationFilter** - Adds Correlation-Id header to all operations
- **JsonOnlyOperationFilter** - Restricts content types to application/json only

### Schema Filters (ISchemaFilter)

Modify data model schemas

- **SwaggerPropsSchemaFilter** - Processes `[SwaggerProps]` attributes for comprehensive schema customization
- **RemoveAdditionalPropertiesFilter** - Removes `additionalProperties` from object schemas

### Document Filters (IDocumentFilter)

Modify the entire OpenAPI document

- **ServersDocumentFilter** - Adds server configurations (dev.local, qa.local)

## SOLID Principles Applied

### Single Responsibility Principle (SRP)

✅ Each filter has one clear, focused responsibility
✅ Service configuration separated from application configuration
✅ Each extension class handles one aspect of Swagger setup

### Open/Closed Principle (OCP)

✅ Easy to add new filters without modifying existing code
✅ Extensible through new filter implementations
✅ Configuration methods are private, extension methods are public

### Liskov Substitution Principle (LSP)

✅ All filters properly implement their respective interfaces
✅ Can be substituted with alternative implementations

### Interface Segregation Principle (ISP)

✅ Each filter implements only the interface methods it needs
✅ No forced dependencies on unused methods
✅ Separate interfaces for different filter types

### Dependency Inversion Principle (DIP)

✅ Filters depend on abstractions (interfaces) not concrete implementations
✅ Configuration is injected through SwaggerGenOptions

## Adding New Filters

### Operation Filter

```csharp
namespace WebApi.StartupExtensions.Swagger.Filters;

public class MyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Your logic here
    }
}
```

Then register in `SwaggerServiceExtensions.AddOperationFilters()`:

```csharp
c.OperationFilter<MyOperationFilter>();
```

### Schema Filter

```csharp
namespace WebApi.StartupExtensions.Swagger.Filters;

public class MySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Your logic here
    }
}
```

Then register in `SwaggerServiceExtensions.AddSchemaFilters()`:

```csharp
c.SchemaFilter<MySchemaFilter>();
```

### Document Filter

```csharp
namespace WebApi.StartupExtensions.Swagger.DocumentFilters;

public class MyDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Your logic here
    }
}
```

Then register in `SwaggerServiceExtensions.AddDocumentFilters()`:

```csharp
c.DocumentFilter<MyDocumentFilter>();
```

## Benefits

1. **Separation of Concerns** - Service config vs Application config
2. **Maintainability** - Each filter is focused and easy to understand
3. **Testability** - Individual filters can be unit tested in isolation
4. **Extensibility** - New filters can be added without modifying existing code
5. **Readability** - Clear folder structure and naming conventions
6. **Reusability** - Filters can be reused across different projects
