INSTRUCTIONS.md
🎯 Goal

Create a .NET 8 Web API project with:

Authors and Books controllers (CRUD)

MediatR for request/response handling

Correlation ID middleware

API Versioning (v1)

Swagger documentation in YAML

StartupExtensions for clean composition

Minimal Program.cs

🧱 Project Structure
/src
└── WebApi/
    ├── Controllers/
    │   └── v1/
    │       ├── AuthorsController.cs
    │       └── BooksController.cs
    │
    ├── Features/
    │   ├── Authors/
    │   │   ├── Models/
    │   │   │   └── AuthorDto.cs
    │   │   ├── Commands/
    │   │   │   ├── CreateAuthorCommand.cs
    │   │   │   ├── UpdateAuthorCommand.cs
    │   │   │   └── DeleteAuthorCommand.cs
    │   │   └── Queries/
    │   │       ├── GetAuthorByIdQuery.cs
    │   │       ├── GetAllAuthorsQuery.cs
    │   │       └── GetBooksByAuthorQuery.cs     // Uses Books feature via MediatR
    │
    │   └── Books/
    │       ├── Models/
    │       │   └── BookDto.cs
    │       ├── Commands/
    │       │   ├── CreateBookCommand.cs
    │       │   ├── UpdateBookCommand.cs
    │       │   └── DeleteBookCommand.cs
    │       └── Queries/
    │           ├── GetBookByIdQuery.cs
    │           ├── GetAllBooksQuery.cs
    │           └── GetBooksByAuthorQuery.cs
    │
    ├── Middleware/
    │   └── CorrelationIdMiddleware.cs
    │
    ├── StartupExtensions/
    │   ├── ServiceCollectionExtensions.cs
    │   ├── ApplicationBuilderExtensions.cs
    │   ├── SwaggerExtensions.cs
    │   ├── MediatRExtensions.cs
    │   ├── ApiVersioningExtensions.cs
    │   └── ValidationExtensions.cs
    │
    ├── appsettings.json
    └── Program.cs

└── WebApi.sln

⚙️ Requirements
1. Controllers

AuthorsController

GET    /api/v1/authors/{id:int}
POST   /api/v1/authors
PUT    /api/v1/authors/{id:int}
DELETE /api/v1/authors/{id:int}
GET    /api/v1/authors/{id:int}/books


BooksController

GET    /api/v1/books
GET    /api/v1/books/{authorId:int}
POST   /api/v1/books
PUT    /api/v1/books/{id:int}
DELETE /api/v1/books/{id:int}


Controllers must:

Use IMediator.Send() for commands/queries

Stay lean — no business logic

Use [ApiVersion("1.0")] and [Route("api/v{version:apiVersion}/[controller]")]

2. Correlation ID Middleware

Reads Correlation-Id from incoming requests

Generates one if missing

Adds it to responses and HttpContext.Items

Register early via UseCorrelationId()

3. Swagger

Use Swashbuckle.AspNetCore

Enable versioned docs: /swagger/v1/swagger.yaml and /swagger/v1/swagger.json

Include request/response examples

Configure in StartupExtensions/SwaggerExtensions.cs

Use YAML output

4. MediatR

Add via NuGet MediatR.Extensions.Microsoft.DependencyInjection

Register with

services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());


Merge request and handler in the same file (e.g. CreateAuthorCommand.cs)

5. API Versioning

Use Asp.Versioning.Http

Register in ApiVersioningExtensions.cs with default v1.0 and URL substitution

6. Startup Extensions and Program.cs

Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddMediatRSetup()
    .AddApiVersioningSetup()
    .AddSwaggerSetup();

var app = builder.Build();

app.UseAppPipeline()
   .UseCorrelationId()
   .UseSwaggerSetup();

app.Run();


StartupExtensions should configure controllers, pipeline, Swagger, versioning, and MediatR cleanly.

7. Expected Behavior

After running:

dotnet run


✅ Available endpoints:

/api/v1/authors

/api/v1/books

Swagger: https://localhost:5001/swagger/v1/swagger.yaml

✅ Behavior:

Each response includes Correlation-Id

Swagger UI displays versioned endpoints

Controllers use MediatR, staying under 20 lines each

8. Optional Enhancements

Add FluentValidation for request DTOs

Add global exception handling (ProblemDetails)

Add Serilog request logging with correlation ID enrichment

✅ Deliverables

Claude or Cursor should generate:

Full project scaffold matching this structure

Controllers wired to MediatR

Correlation ID middleware

API versioning and Swagger YAML setup

Clean, minimal Program.cs