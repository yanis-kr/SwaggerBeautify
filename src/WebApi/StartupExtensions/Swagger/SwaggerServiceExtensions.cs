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
            // Apply all configurations using the strategy pattern
            var configurations = new List<ISwaggerConfiguration>
            {
                new SwaggerDocumentConfiguration(),
                new SwaggerSecurityConfiguration(),
                new SwaggerXmlCommentsConfiguration(),
                new SwaggerFiltersConfiguration(),
                new SwaggerTypeMappingsConfiguration()
            };

            foreach (var configuration in configurations)
            {
                configuration.Configure(options);
            }
        });

        return services;
    }
}
