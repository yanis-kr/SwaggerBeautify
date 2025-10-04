using WebApi.Middleware;
using WebApi.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddMediatRSetup()
    .AddApiVersioningSetup()
    .AddSwaggerSetup();

var app = builder.Build();

app.UseCorrelationId()
    .UseAppPipeline()
    .UseSwaggerSetup();

app.MapControllers();
app.Run();
