# WebApi - Swagger/OpenAPI Demo Project

A comprehensive ASP.NET Core Web API demonstrating best practices for Swagger/OpenAPI documentation with SOLID architecture principles.

## How to Copy This Swagger Setup to Your Project

To integrate this comprehensive Swagger configuration into your own project:

### Step 1: Copy the SwaggerProps Attribute

Copy `src/WebApi/Attributes/SwaggerPropsAttribute.cs` to your project:

- **Recommended location**: Domain project or shared infrastructure project that your models reference
- **Purpose**: Enables rich Swagger documentation via attributes on your DTOs/models

### Step 2: Copy the Swagger Configuration

Copy the entire `src/WebApi/StartupExtensions/Swagger/` folder to your project:

- **Location**: `YourProject/StartupExtensions/Swagger/`
- **Contains**: All filters, configurations, and extension methods
- **Includes**:
  - Configuration classes (document, security, filters, type mappings, XML comments)
  - Custom filters (CorrelationId, JsonOnly, SwaggerProps, RemoveAdditionalProperties)
  - Document filters (Servers configuration)

### Step 3: Register in Program.cs

Add these lines to your `Program.cs`:

```csharp
using YourProject.StartupExtensions.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure Swagger UI
app.UseSwaggerDocumentation();

app.MapControllers();
app.Run();
```

### Step 4: Enable XML Documentation (Optional but Recommended)

Add to your `.csproj` file:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

That's it! You'll have a fully-featured Swagger setup with JWT security, custom attributes, and comprehensive documentation.

## Quick Start

### Build and Run

```bash
# Build solution
dotnet build

# Run API
dotnet run --project src/WebApi

# Navigate to Swagger UI
# https://localhost:60983/swagger
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage report (opens in Edge)
.\run-coverage.ps1
```

## Features

### 🚀 Custom Mediator Implementation

- ✅ **Zero Dependencies** - No MediatR package required
- ✅ **Lightweight** - ~150 lines of simple code
- ✅ **Auto-Registration** - Handlers discovered automatically
- ✅ **Type-Safe** - Compile-time request/response validation
- ✅ **Fully Tested** - 24 unit tests covering all scenarios

### 🎯 Swagger/OpenAPI Configuration

- ✅ **SOLID Architecture** - Modular, single-responsibility classes
- ✅ **JWT Bearer Security** - Comprehensive authentication setup
- ✅ **Custom Attributes** - `[SwaggerProps]` for rich documentation
- ✅ **XML Documentation** - Automatic API documentation from code comments
- ✅ **Custom Filters** - Operation, schema, and document filters
- ✅ **Type Mappings** - Consistent JSON schema generation
- ✅ **Server Configurations** - Dev and QA environments
- ✅ **JSON-Only API** - Restricted to `application/json` content type
- ✅ **Correlation ID** - Request tracking header

### 🧪 Comprehensive Testing

- **86 Unit Tests** - Full coverage of Swagger and Mediator implementations
- **Test Coverage Reporting** - HTML reports with line/branch coverage
- **FluentAssertions** - Readable, expressive test assertions
- **NSubstitute** - Clean mocking for dependencies

### 📐 Architecture Highlights

#### SOLID Principles

- **Single Responsibility** - Each class has one job
- **Open/Closed** - Easy to extend, no modification needed
- **Pragmatic Design** - No over-engineering

#### Features Organization

- **Vertical Slices** - Features organized by domain (Authors, Books)
- **CQRS Pattern** - Commands and Queries separated
- **MediatR** - Request/response pattern

## Key Components

### Custom Attribute: SwaggerProps

```csharp
public class AuthorDto
{
    [SwaggerProps(Example = 1)]
    public int Id { get; set; }

    [SwaggerProps(Example = "John Doe")]
    public string Name { get; set; }

    [SwaggerProps(Example = "john.doe@example.com", Format = "email")]
    public string Email { get; set; }

    [SwaggerProps(Example = "2024-01-15T10:30:00Z", Format = "date-time", ReadOnly = true)]
    public DateTime CreatedAt { get; set; }

    [SwaggerProps(Hide = true)]
    public string InternalTrackingId { get; set; }
}
```

### Swagger Configuration Classes

```csharp
// Document metadata
SwaggerDocumentConfiguration.Configure(options);

// JWT Bearer security
SwaggerSecurityConfiguration.Configure(options);

// XML comments
SwaggerXmlCommentsConfiguration.Configure(options);

// Filters registration
SwaggerFiltersConfiguration.Configure(options);

// Type mappings
SwaggerTypeMappingsConfiguration.Configure(options);
```

### Custom Filters

- **CorrelationIdOperationFilter** - Adds tracking header to all endpoints
- **JsonOnlyOperationFilter** - Restricts to JSON content type
- **SwaggerPropsSchemaFilter** - Processes `[SwaggerProps]` attributes
- **RemoveAdditionalPropertiesFilter** - Cleans up schema output
- **ServersDocumentFilter** - Adds environment server configurations

## Documentation

- **[COVERAGE.md](COVERAGE.md)** - Test coverage guide
- **[src/WebApi/Infrastructure/Mediator/README.md](src/WebApi/Infrastructure/Mediator/README.md)** - Custom mediator implementation
- **[src/WebApi/StartupExtensions/Swagger/README.md](src/WebApi/StartupExtensions/Swagger/README.md)** - Swagger architecture
- **[src/WebApi/StartupExtensions/Swagger/Configuration/README.md](src/WebApi/StartupExtensions/Swagger/Configuration/README.md)** - Configuration details
- **[tests/WebApi.Tests/README.md](tests/WebApi.Tests/README.md)** - Test suite documentation

## Technologies

- **.NET 8.0** - API runtime
- **.NET 9.0** - Test runtime
- **ASP.NET Core** - Web framework
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI generation
- **Custom Mediator** - Lightweight CQRS implementation (no dependencies!)
- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library
- **NSubstitute** - Mocking framework
- **Coverlet** - Code coverage
- **ReportGenerator** - Coverage report generation

## API Endpoints

### Authors (v1)

- `GET /api/v1/authors` - Get all authors
- `GET /api/v1/authors/{id}` - Get author by ID
- `GET /api/v1/authors/{id}/books` - Get books by author
- `POST /api/v1/authors` - Create author
- `PUT /api/v1/authors/{id}` - Update author
- `DELETE /api/v1/authors/{id}` - Delete author

### Books (v1)

- `GET /api/v1/books` - Get all books
- `GET /api/v1/books/{id}` - Get book by ID
- `POST /api/v1/books` - Create book
- `PUT /api/v1/books/{id}` - Update book
- `DELETE /api/v1/books/{id}` - Delete book

## Development

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / Rider
- Edge browser (for coverage reports)

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project src/WebApi
```

### Test

```bash
# Run tests
dotnet test

# Run tests with coverage
.\run-coverage.ps1
```

## OpenAPI Specification

- **Version**: 1.2.3
- **Format**: YAML
- **Endpoint**: `/swagger/v1/swagger.yaml`
- **UI**: `/swagger` (Swagger UI with custom configuration)

## Contributing

When adding new features:

1. Follow SOLID principles
2. Add comprehensive unit tests
3. Update XML documentation
4. Use `[SwaggerProps]` for API documentation
5. Maintain >85% test coverage

## License

MIT License - See project for details

## Contact

For questions or issues, refer to the documentation in the respective README files.
