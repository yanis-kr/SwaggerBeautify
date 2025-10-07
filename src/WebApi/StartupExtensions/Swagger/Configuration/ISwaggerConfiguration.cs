using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Configuration;

/// <summary>
/// Interface for Swagger configuration components
/// </summary>
public interface ISwaggerConfiguration
{
    /// <summary>
    /// Applies configuration to SwaggerGenOptions
    /// </summary>
    void Configure(SwaggerGenOptions options);
}

