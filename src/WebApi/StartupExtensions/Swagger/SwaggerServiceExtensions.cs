using WebApi.StartupExtensions.Swagger.Configuration;

namespace WebApi.StartupExtensions.Swagger;

/// <summary>
/// Extension methods for configuring Swagger services (builder.Services)
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Adds Swagger services and configuration to the service collection
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Apply configurations in order
            SwaggerDocumentConfiguration.Configure(options);
            SwaggerSecurityConfiguration.Configure(options);
            SwaggerXmlCommentsConfiguration.Configure(options);
            SwaggerFiltersConfiguration.Configure(options);
            SwaggerTypeMappingsConfiguration.Configure(options);
        });

        return services;
    }
}
