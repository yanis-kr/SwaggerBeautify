# Custom Mediator Implementation

A lightweight, simple mediator pattern implementation for request/response handling. This is a custom alternative to Jimmy Bogard's MediatR library.

## Why Custom Implementation?

- ✅ **Zero Dependencies** - No external packages required
- ✅ **Lightweight** - ~150 lines of code total
- ✅ **Simple** - Easy to understand and maintain
- ✅ **Full Control** - Customize behavior as needed
- ✅ **Same API** - Compatible with MediatR patterns

## Components

### Core Interfaces

```
Infrastructure/Mediator/
├── IMediator.cs              # Mediator interface
├── IRequest.cs               # Request marker interfaces
├── IRequestHandler.cs        # Handler interfaces
├── Mediator.cs               # Mediator implementation
└── Unit.cs                   # Void return type
```

### 1. IRequest<TResponse>

Marker interface for requests that return a response.

```csharp
public interface IRequest<out TResponse> { }

// Requests without return value
public interface IRequest : IRequest<Unit> { }
```

### 2. IRequestHandler<TRequest, TResponse>

Interface for request handlers.

```csharp
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

// Handlers without return value
public interface IRequestHandler<in TRequest>
    : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit> { }
```

### 3. IMediator

Main mediator interface for sending requests.

```csharp
public interface IMediator
{
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
```

### 4. Mediator

Implementation that uses DI to resolve and invoke handlers.

```csharp
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        // Resolves IRequestHandler<TRequest, TResponse> from DI
        // Invokes Handle method
        // Returns result
    }
}
```

### 5. Unit

Represents a void return type (similar to MediatR's Unit).

```csharp
public struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = new();
}
```

## Usage

### Define a Query

```csharp
// Query that returns data
public record GetAuthorByIdQuery(int Id) : IRequest<AuthorDto?>;

// Handler
public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, AuthorDto?>
{
    public Task<AuthorDto?> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        // Query logic here
        var author = _repository.GetById(request.Id);
        return Task.FromResult(author);
    }
}
```

### Define a Command

```csharp
// Command that doesn't return data
public record DeleteAuthorCommand(int Id) : IRequest;

// Handler (returns Unit)
public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand>
{
    public Task<Unit> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        // Delete logic here
        _repository.Delete(request.Id);
        return Task.FromResult(Unit.Value);
    }
}
```

### Use in Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var author = await mediator.Send(new GetAuthorByIdQuery(id));
        return author != null ? Ok(author) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteAuthorCommand(id));
        return NoContent();
    }
}
```

### Register in Program.cs

```csharp
builder.Services.AddMediatorSetup();
```

This automatically:

- Registers `IMediator` as scoped
- Scans assembly for all `IRequestHandler<,>` implementations
- Registers all handlers as scoped services

## Features

### ✅ Automatic Handler Registration

No need to manually register each handler - they're discovered automatically:

```csharp
services.AddMediatorSetup(); // Scans and registers all handlers
```

### ✅ Constructor Injection

Handlers support constructor injection for dependencies:

```csharp
public class CreateAuthorCommandHandler(
    IRepository repository,
    ILogger<CreateAuthorCommandHandler> logger)
    : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    public async Task<AuthorDto> Handle(...)
    {
        logger.LogInformation("Creating author");
        return await repository.CreateAsync(...);
    }
}
```

### ✅ Cancellation Token Support

All handlers receive a cancellation token:

```csharp
public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
{
    await _repository.SaveAsync(author, cancellationToken);
}
```

### ✅ Type Safety

Compile-time type checking for requests and responses:

```csharp
// ✅ Type-safe
var author = await mediator.Send(new GetAuthorByIdQuery(1)); // Returns AuthorDto?

// ❌ Won't compile
int result = await mediator.Send(new GetAuthorByIdQuery(1)); // Type mismatch
```

## Comparison: Custom vs MediatR

| Feature                  | Custom Implementation | MediatR Library      |
| ------------------------ | --------------------- | -------------------- |
| **Dependencies**         | Zero                  | MediatR + Extensions |
| **Lines of Code**        | ~150                  | Thousands            |
| **Handler Registration** | Auto-scan             | Auto-scan            |
| **Request/Response**     | ✅                    | ✅                   |
| **Unit Type**            | ✅                    | ✅                   |
| **Behaviors/Pipeline**   | ❌ (not needed)       | ✅                   |
| **Notifications**        | ❌ (not needed)       | ✅                   |
| **Stream Support**       | ❌ (not needed)       | ✅                   |
| **Complexity**           | Simple                | Complex              |
| **Learning Curve**       | Minutes               | Hours                |

## What's NOT Included (vs MediatR)

This implementation focuses on the core request/response pattern and **intentionally omits**:

- ❌ **Pipeline Behaviors** - Use middleware or filters instead
- ❌ **Notifications/Events** - Use a proper event bus if needed
- ❌ **Stream Requests** - Not commonly needed
- ❌ **Request Pre/Post Processors** - Use middleware
- ❌ **Complex Configuration** - Keep it simple

**Philosophy**: If you need these features, use the full MediatR library. For simple request/response, this is perfect.

## Testing

The implementation is fully tested with 18 tests covering:

- ✅ Request handling with responses
- ✅ Request handling without responses (Unit)
- ✅ Handler resolution from DI
- ✅ Cancellation token passing
- ✅ Multiple handler resolution
- ✅ Error handling for missing handlers
- ✅ Null request validation
- ✅ Unit type behavior

### Example Test

```csharp
[Fact]
public async Task Send_WithValidHandler_ShouldReturnResponse()
{
    var services = new ServiceCollection();
    services.AddScoped<IRequestHandler<MyQuery, string>, MyQueryHandler>();
    services.AddScoped<IMediator, Mediator>();

    var mediator = services.BuildServiceProvider()
        .GetRequiredService<IMediator>();

    var result = await mediator.Send(new MyQuery("test"));

    result.Should().Be("Response: test");
}
```

## When to Use

### ✅ Use Custom Implementation When:

- Simple CQRS pattern needed
- No pipeline behaviors required
- Want zero dependencies
- Prefer simplicity over features
- Easy to understand codebase

### ❌ Use MediatR Library When:

- Need pipeline behaviors (validation, logging, etc.)
- Need notification/event support
- Need streaming requests
- Large team familiar with MediatR
- Complex cross-cutting concerns

## Performance

The custom implementation is lightweight:

- **Handler Resolution**: O(1) - Direct DI container lookup
- **Memory**: Minimal - No pipeline, no behaviors
- **Startup**: Fast - Simple reflection scan

## Migration from MediatR

If you're migrating from MediatR:

1. Replace `using MediatR;` with `using WebApi.Infrastructure.Mediator;`
2. Replace `services.AddMediatR(...)` with `services.AddMediatorSetup()`
3. Remove MediatR NuGet packages
4. Build and test - API is compatible!

### Breaking Changes

- ❌ No `IPipelineBehavior<,>`
- ❌ No `INotification` / `INotificationHandler`
- ❌ No `IStreamRequest<>`
- ❌ No `IRequestPreProcessor<>` / `IRequestPostProcessor<>`

If you use these, stick with MediatR.

## Design Decisions

### Why Reflection for Handler Resolution?

- **Simplicity** - No need for compiled expressions
- **Performance** - Handler is cached by DI container
- **Testability** - Easy to mock and test

### Why Scoped Lifetime?

- **Request Context** - New instance per HTTP request
- **Safety** - Prevents state sharing between requests
- **Standard** - Matches typical handler lifetime

### Why Auto-Registration?

- **Convention over Configuration** - Handlers are discovered automatically
- **Less Boilerplate** - No manual registration needed
- **Maintainable** - Add handler, it's automatically available

## Contributing

When adding features to the mediator:

1. Keep it simple - Don't replicate all MediatR features
2. Add tests for new functionality
3. Update this README
4. Consider if the feature is truly needed

## License

Part of the WebApi project - MIT License
