using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.DocumentFilters;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.StartupExtensions.Swagger.Configuration;

/// <summary>
/// Configures Swagger operation, schema, and document filters
/// </summary>
public static class SwaggerFiltersConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        // Operation filters
        options.OperationFilter<JsonOnlyOperationFilter>();

        // Parameter filters
        options.ParameterFilter<SwaggerPropsParameterFilter>();

        // Schema filters
        options.SchemaFilter<SwaggerPropsSchemaFilter>();
        options.SchemaFilter<RemoveAdditionalPropertiesFilter>();

        // Document filters
        options.DocumentFilter<ServersDocumentFilter>();
    }
}

