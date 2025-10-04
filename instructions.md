🎯 Goal

Create a .NET 8 Web API project following modern professional standards with:

Authors and Books controllers (CRUD)

MediatR for application flow

Correlation ID propagation

API Versioning (v1)

Swagger documentation in YAML

StartupExtensions for clean composition

Minimal Program.cs

🧱 Project Structure
/src
├── WebApi
│ ├── Controllers/
│ │ ├── v1/
│ │ │ ├── AuthorsController.cs
│ │ │ └── BooksController.cs
│ ├── Features/
│ │ ├── Authors/
│ │ │ ├── Commands/
│ │ │ │ ├── CreateAuthorCommand.cs
│ │ │ │ ├── UpdateAuthorCommand.cs
│ │ │ │ └── DeleteAuthorCommand.cs
│ │ │ └── Queries/
│ │ │ ├── GetAuthorByIdQuery.cs
│ │ │ └── GetAllAuthorsQuery.cs
│ │ └── Books/
│ │ ├── Commands/
│ │ ├── Queries/
│ ├── Models/
│ │ ├── Authors/
│ │ │ ├── Author.cs
│ │ │ ├── CreateAuthorRequest.cs
│ │ │ └── UpdateAuthorRequest.cs
│ │ └── Books/
│ │ ├── Book.cs
│ │ ├── CreateBookRequest.cs
│ │ └── UpdateBookRequest.cs
│ ├── Middleware/
│ │ └── CorrelationIdMiddleware.cs
│ ├── StartupExtensions/
│ │ ├── ServiceCollectionExtensions.cs
│ │ ├── ApplicationBuilderExtensions.cs
│ │ ├── SwaggerExtensions.cs
│ │ ├── MediatRExtensions.cs
│ │ └── ApiVersioningExtensions.cs
│ └── Program.cs
└── WebApi.sln

⚙️ Requirements

1. Controllers
   AuthorsController

Routes:

GET /api/v1/authors/{id:int}

POST /api/v1/authors

PUT /api/v1/authors/{id:int}

DELETE /api/v1/authors/{id:int}

BooksController

Routes:

GET /api/v1/books

GET /api/v1/books/{authorId:int}

POST /api/v1/books

PUT /api/v1/books/{id:int}

DELETE /api/v1/books/{id:int}

Notes

Each action should use IMediator.Send() to dispatch commands or queries.

Controllers stay thin: no business logic.

Apply [ApiVersion("1.0")] and [Route("api/v{version:apiVersion}/[controller]")].

Example:

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
[HttpGet("{id:int}")]
public async Task<IActionResult> Get(int id) =>
Ok(await mediator.Send(new GetAuthorByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAuthorRequest request)
    {
        var author = await mediator.Send(new CreateAuthorCommand(request));
        return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateAuthorRequest request)
    {
        await mediator.Send(new UpdateAuthorCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteAuthorCommand(id));
        return NoContent();
    }

}

2. Correlation ID Middleware

Implement a custom middleware that:

Reads Correlation-Id header from the incoming request.

If missing, generates a new GUID.

Adds the same header to the response.

Makes correlation ID available via HttpContext.Items.

Register early in the pipeline (UseCorrelationId()).

3. Swagger Configuration

Use Swashbuckle.AspNetCore.

Enable versioned docs (/swagger/v1/swagger.yaml and /swagger/v1/swagger.json).

Show request/response examples.

Include [ProducesResponseType] annotations.

Enable YAML output with:

c.SerializeAsV2 = false;
c.IncludeYaml();

Move all Swagger setup into StartupExtensions/SwaggerExtensions.cs.

4. MediatR

Add MediatR via NuGet (MediatR.Extensions.Microsoft.DependencyInjection).

Register handlers with AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

Keep each command/query in Features folders.

Example command:

public record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<Author>;

5. API Versioning

Add Asp.Versioning.Http package.

Register in ApiVersioningExtensions.cs:

services.AddApiVersioning(options =>
{
options.DefaultApiVersion = new ApiVersion(1, 0);
options.AssumeDefaultVersionWhenUnspecified = true;
options.ReportApiVersions = true;
});
services.AddVersionedApiExplorer(options =>
{
options.GroupNameFormat = "'v'VVV";
options.SubstituteApiVersionInUrl = true;
});

6. Startup Extension Registration
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

StartupExtensions/ServiceCollectionExtensions.cs

Register controllers, API behavior, etc.

StartupExtensions/ApplicationBuilderExtensions.cs

Configure middleware order (exception handler, correlation ID, routing, etc.)

7. Expected Behavior

After running:

dotnet run

✅ Endpoints:

/api/v1/authors

/api/v1/books

Swagger: https://localhost:5001/swagger/v1/swagger.yaml

✅ Behavior:

Each response includes Correlation-Id.

Swagger UI displays versioned endpoints.

Controllers remain under 20 lines, using MediatR for business flow.

8. Optional Enhancements

Add FluentValidation for request DTOs.

Add global exception handling middleware returning ProblemDetails.

Add Serilog request logging with correlation ID enrichment.

✅ Deliverables for Claude/Cursor

Claude or Cursor should generate:

Full project scaffold following this structure.

Controllers using MediatR.

Versioned routing with Swagger YAML.

Middleware and StartupExtensions setup.

Clean, minimal Program.cs.
