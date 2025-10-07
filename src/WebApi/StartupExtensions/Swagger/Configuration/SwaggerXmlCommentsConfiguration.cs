using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.StartupExtensions.Swagger.Configuration;

/// <summary>
/// Configures XML documentation comments for Swagger
/// </summary>
public class SwaggerXmlCommentsConfiguration : ISwaggerConfiguration
{
    public void Configure(SwaggerGenOptions options)
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
}

