# Common Parameters Refactoring Summary

## Overview

Successfully refactored the Swagger/OpenAPI configuration to use **individual header parameters** in controller actions instead of a global operation filter. Each header parameter (`Correlation-Id` and `User-Context`) appears separately in Swagger UI and is passed as a standard HTTP header (NOT as JSON).

## Key Achievement

✅ **Individual HTTP headers** - Clients send separate headers like `Correlation-Id: guid` and `User-Context: string`, not JSON objects  
✅ **Accessible in commands/queries** - Headers are wrapped in `CommonParameters` and passed through to business logic  
✅ **Rich Swagger documentation** - Each header parameter appears with examples and descriptions in OpenAPI spec

## Changes Made

### 1. Created CommonParameters Class

**File:** `src/WebApi/Models/CommonParameters.cs`

- Simple container class for passing header values to commands/queries
- Properties:
  - `CorrelationId` (Guid?) - For request tracking across services
  - `UserContext` (string?) - For user identification and authorization
- Constructor accepts both parameters for easy instantiation in controllers

### 2. Created SwaggerPropsParameterFilter

**File:** `src/WebApi/StartupExtensions/Swagger/Filters/SwaggerPropsParameterFilter.cs`

- Implements `IParameterFilter` to process `SwaggerProps` attributes on action parameters
- Applies example values, descriptions, formats, and other metadata to OpenAPI parameters
- Works alongside the existing `SwaggerPropsSchemaFilter` for comprehensive documentation
- Includes 4 passing unit tests

### 3. Updated All Commands and Queries

Updated 12 files to accept `CommonParameters`:

**Books Feature:**

- `CreateBookCommand.cs`
- `UpdateBookCommand.cs`
- `DeleteBookCommand.cs`
- `GetAllBooksQuery.cs`
- `GetBookByIdQuery.cs`
- `GetBooksByAuthorQuery.cs`

**Authors Feature:**

- `CreateAuthorCommand.cs`
- `UpdateAuthorCommand.cs`
- `DeleteAuthorCommand.cs`
- `GetAllAuthorsQuery.cs`
- `GetAuthorByIdQuery.cs`
- `GetBooksByAuthorQuery.cs`

All commands and queries now receive `CommonParameters` as part of their constructor, making header values accessible in business logic.

### 4. Updated All Controller Actions

**Files:**

- `src/WebApi/Controllers/v1/BooksController.cs`
- `src/WebApi/Controllers/v1/AuthorsController.cs`

Each action now accepts **individual header parameters** with `SwaggerProps` attributes:

```csharp
[FromHeader(Name = "Correlation-Id")]
[SwaggerProps(Format = "uuid", Example = "12345678-1234-1234-1234-123456789abc")]
Guid? correlationId,

[FromHeader(Name = "User-Context")]
[SwaggerProps(Example = "user@example.com|Admin")]
string? userContext
```

Controllers construct `CommonParameters` object from these individual headers and pass it to commands/queries.

### 5. Updated Swagger Configuration

**File:** `src/WebApi/StartupExtensions/Swagger/Configuration/SwaggerFiltersConfiguration.cs`

- Removed `CorrelationIdOperationFilter` registration
- Added `SwaggerPropsParameterFilter` registration

### 6. Removed Obsolete Code

- Deleted `src/WebApi/StartupExtensions/Swagger/Filters/CorrelationIdOperationFilter.cs`
- Deleted `tests/WebApi.Tests/Filters/CorrelationIdOperationFilterTests.cs`

### 7. Added New Tests

**File:** `tests/WebApi.Tests/Filters/SwaggerPropsParameterFilterTests.cs`

- Created 4 comprehensive tests for the new parameter filter
- All tests passing ✓

## Benefits

### 1. **Individual Header Parameters (Not JSON)**

✅ Each parameter is sent as a separate HTTP header:

- Request Header: `Correlation-Id: 12345678-1234-1234-1234-123456789abc`
- Request Header: `User-Context: user@example.com|Admin`

**No JSON object required** - just standard HTTP headers that clients can easily set and that appear individually in Swagger UI.

### 2. **Access to Common Parameters in Business Logic**

Commands and queries can now access correlation IDs and user context directly:

```csharp
public Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
{
    var correlationId = request.CommonParams.CorrelationId ?? Guid.NewGuid();
    var userContext = request.CommonParams.UserContext;

    // Use these values in business logic, logging, database calls, etc.
    // ...
}
```

### 3. **Explicit Parameter Declaration**

Parameters are explicitly declared as individual headers in controller actions, making the API contract crystal clear to consumers.

### 4. **Rich OpenAPI Documentation**

The OpenAPI specification now includes:

- ✅ Example values for all header parameters (visible in Swagger UI)
- ✅ Detailed descriptions explaining their purpose
- ✅ Proper format specifications (e.g., `uuid` for Correlation-Id)
- ✅ Each parameter appears as a separate input field in Swagger UI

### 5. **Type Safety**

`Correlation-Id` is now typed as `Guid?` instead of string, providing compile-time type checking.

### 6. **Extensibility**

Easy to add more common parameters:

1. Add a new parameter to controller actions with `[FromHeader]` and `SwaggerProps`
2. Add it to the `CommonParameters` constructor
3. Update constructor calls in controllers
4. Use it in commands/queries

## Test Results

- **Total Tests:** 85
- **Passing:** 82 ✓
- **Failing:** 3 (pre-existing ServersDocumentFilterTests issues, unrelated to this refactoring)
- **New Tests Added:** 4 for SwaggerPropsParameterFilter ✓

## Build Status

- ✓ WebApi project builds successfully with 0 warnings
- ✓ WebApi.Tests project builds successfully with 0 warnings
- ✓ No linting errors

## Usage Example

### In Controllers:

```csharp
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetAll(
    [FromHeader(Name = "Correlation-Id")]
    [SwaggerProps(Format = "uuid", Example = "12345678-1234-1234-1234-123456789abc",
        Description = "Optional correlation ID for request tracking")]
    Guid? correlationId,
    [FromHeader(Name = "User-Context")]
    [SwaggerProps(Example = "user@example.com|Admin",
        Description = "User context containing user identification and role information")]
    string? userContext)
{
    var commonParams = new CommonParameters(correlationId, userContext);
    var books = await mediator.Send(new GetAllBooksQuery(commonParams));
    return Ok(books);
}
```

### In Commands/Queries:

```csharp
public record GetAllBooksQuery(CommonParameters CommonParams) : IRequest<IEnumerable<BookDto>>;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    public Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        // Access parameters:
        var correlationId = request.CommonParams.CorrelationId;
        var userContext = request.CommonParams.UserContext;

        // Use in logging, business logic, etc.
        return Task.FromResult(_books.AsEnumerable());
    }
}
```

### How Clients Use the API:

Clients send standard HTTP headers with each request:

```bash
curl -X GET "https://api.example.com/api/v1/books" \
  -H "Correlation-Id: 12345678-1234-1234-1234-123456789abc" \
  -H "User-Context: user@example.com|Admin"
```

### In OpenAPI/Swagger UI:

Each parameter appears as a separate input field:

**Parameter: Correlation-Id**

- Location: Header
- Type: `string` with format `uuid`
- Example: `12345678-1234-1234-1234-123456789abc`
- Description: Optional correlation ID for request tracking
- Required: No

**Parameter: User-Context**

- Location: Header
- Type: `string`
- Example: `user@example.com|Admin`
- Description: User context containing user identification and role information
- Required: No

## Migration Notes

✅ No breaking changes for API consumers - the header parameters work exactly as expected  
✅ Headers are optional and work as standard HTTP headers (not JSON)  
✅ Internal code now passes `CommonParameters` to all commands and queries  
✅ Headers are accessible throughout the application stack
