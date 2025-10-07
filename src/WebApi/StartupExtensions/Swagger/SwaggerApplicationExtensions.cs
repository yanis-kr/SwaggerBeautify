using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApi.StartupExtensions.Swagger;

/// <summary>
/// Extension methods for configuring Swagger middleware (app.*)
/// </summary>
public static class SwaggerApplicationExtensions
{
    /// <summary>
    /// Configures Swagger UI with enhanced settings
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            ConfigureSwaggerUI(app, c);
        });

        return app;
    }

    #region Private Configuration Methods

    private static void ConfigureSwaggerUI(IApplicationBuilder app, SwaggerUIOptions c)
    {
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions.Reverse())
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.yaml",
                $"WebApi {description.GroupName.ToUpperInvariant()}");
        }

        c.RoutePrefix = "swagger";

        // Enhanced Swagger UI configuration
        c.EnableValidator(); // Enable validator badge
    }

    #endregion
}

