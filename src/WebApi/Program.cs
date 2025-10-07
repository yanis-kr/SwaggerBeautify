using WebApi.Middleware;
using WebApi.StartupExtensions;
using WebApi.StartupExtensions.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddMediatRSetup()
    .AddApiVersioningSetup()
    .AddSwaggerDocumentation();

var app = builder.Build();

app.UseCorrelationId()
    .UseAppPipeline()
    .UseSwaggerDocumentation();

app.MapControllers();
app.Run();
