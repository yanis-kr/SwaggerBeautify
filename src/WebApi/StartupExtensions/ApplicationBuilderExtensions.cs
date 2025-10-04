namespace WebApi.StartupExtensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppPipeline(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
